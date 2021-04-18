using GK.WebScraping.Model;
using GK.WebScraping.Model.Code.Collections.GK.WebScraping.Model.Collection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GK.WebScraping.DB
{
    public class DatabaseTransactionQueueManager
    {
        /// <summary>
        /// Transaction queues holds all transactions to be processed in order. Write transactions should always be prioritised. Therefore we are implementing a custom comparer to prioritise those.
        /// Key: ThreadID,
        /// Value : Transaction
        /// </summary>
        private volatile PriorityQueue<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>> _queue;
        private volatile Dictionary<Guid, WebScrapingContext> _activeContexts;
        private static readonly SemaphoreSlim asyncLock = new SemaphoreSlim(1);
        private static readonly object _lock = new object();
        private ILogger _logger;

        #region Singleton
        private static volatile DatabaseTransactionQueueManager _instance;
        private int _numberOfThread;

        public static DatabaseTransactionQueueManager Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new DatabaseTransactionQueueManager();

                return _instance;
            }
        }
        #endregion

        public DatabaseTransactionQueueManager()
        {
            if (this._queue == null)
                _queue = new PriorityQueue<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>>(new DatabaseOperationPriorityComparer());

            if (this._activeContexts == null)
                _activeContexts = new Dictionary<Guid, WebScrapingContext>();

        }


        public void InitLogger(ILogger logger)
        {
            this._logger = logger;
        }

        private void WriteLog(String type, String format, params Object[] args)
        {
            if (this._logger != null)
                switch (type.ToLower())
                {
                    case "info":
                    case "information":
                        this._logger.LogInformation(format, args);
                        break;
                    case "warn":
                    case "warning":
                        this._logger.LogWarning(format, args);
                        break;
                    case "err":
                    case "fail":
                    case "error":
                        this._logger.LogError(format, args);
                        break;
                    default:
                        break;
                }
        }


        private void WriteLog(Exception ex)
        {
            if (this._logger != null)
                this._logger.LogError(ex, "{0} \n {1}", ex.Message, ex.StackTrace);
        }

        public WebScrapingContext GetContext(DatabaseProcessKey key, DbContextOptions<WebScrapingContext> options = null)
        {
            if (this._activeContexts.ContainsKey(key.OperationID) == false)
                this.InitThreadContext(key.OperationID, options);
            return this._activeContexts[key.OperationID];
        }

        public async void QueueTransaction(DatabaseProcessKey key, IDbContextTransaction transaction)
        {
            if (this._activeContexts.TryGetValue(key.OperationID, out WebScrapingContext context) == false)
                throw new Exception("No active context could be found for this thread. Please define database context before queuing a transaction");


            //Save changes before enqueue
            context.SaveChanges();

            this._queue.Enqueue(new KeyValuePair<DatabaseProcessKey, IDbContextTransaction>(key, transaction));


            if (this._numberOfThread == 0)
                this.ProcessAll();
        }

        private void InitThreadContext(Guid operationID, DbContextOptions<WebScrapingContext> options = null)
        {

            WebScrapingContext newContext = options == null ? new WebScrapingContext() : new WebScrapingContext(options);
            newContext.Database.AutoTransactionsEnabled = false;
            this._activeContexts.Add(operationID, newContext);
        }


        /// <summary>
        /// Enqueues the next item from the priority queue and processes it.
        /// </summary>
        /// <returns>Boolean success</returns>
        public async Task<Boolean> ProcessItemAsync(KeyValuePair<DatabaseProcessKey, IDbContextTransaction> process)
        {
            if (this._activeContexts.TryGetValue(process.Key.OperationID, out WebScrapingContext context) == false)
                throw new Exception(String.Format("Could not find any active context for thread '{0}'", process.Key.OperationID.ToString()));

            //Do not dispose context in using scope. Only transaction needs to be disposed at this point.

            using (context)
            using (IDbContextTransaction transaction = process.Value)
            {
                try
                {
                    await transaction.CreateSavepointAsync("Before");

                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackToSavepointAsync("Before");
                    return false;
                }

            }


        }

        private async Task ProcessAll()
        {
            // Lock this up so that one thread at a time can get through here.  
            // Others will do an async await until it is their turn.
            await asyncLock.WaitAsync();
            try
            {
                // Offload this to a background thread (so that the UI is not affected)
                var queueProcessingTask = Task.Run(() =>
                {
                    this._numberOfThread++;
                    var isSuccessful = false;
                    while (this._queue.Count >= 1 && !isSuccessful)
                    {
                        // Get the next item
                        var nextItem = this._queue.NextItem;
                        // Try to process this one. (ie DoStuff)
                        isSuccessful = ProcessItemAsync(nextItem).GetAwaiter().GetResult();
                        // If we processed successfully, then we can dequeue the item
                        if (isSuccessful)
                        {
                            this._queue.Pop();
                            this.DumpThreadContext(nextItem.Key.OperationID);
                        }
                    }
                    this._numberOfThread--;
                });

            }
            finally
            {
                asyncLock.Release();
            }
        }

        /// <summary>
        /// Dumps database context belonging to the thread. This should be called from Thread.Stop method.
        /// </summary>
        /// <param name="threadId"></param>
        public void DumpThreadContext(Guid threadId)
        {
            this._activeContexts[threadId].DisposeAsync();
            this._activeContexts.Remove(threadId);
        }
    }

    public class DatabaseOperationPriorityComparer : IComparer<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>>
    {
        public int Compare(KeyValuePair<DatabaseProcessKey, IDbContextTransaction> p1, KeyValuePair<DatabaseProcessKey, IDbContextTransaction> p2)
        {
            int x = (Int32)p1.Key.Priority;
            int y = (Int32)p2.Key.Priority;

            if (x == y)
                return 0;
            else if (x > y)
                return 1;
            else
                return -1;
        }
    }
}
