using GK.WebScraping.DB;
using GK.WebScraping.Model;
using GK.WebScraping.Model.Code.Collections.GK.WebScraping.Model.Collection;
using GK.WebScraping.Utilities.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GK.WebScraping.Utilities.Queues
{
    public class DatabaseTransactionQueue : ApplicationQueueBase<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>>
    {
        /// <summary>
        /// Transaction queues holds all transactions to be processed in order. Write transactions should always be prioritised. Therefore we are implementing a custom comparer to prioritise those.
        /// Key: ThreadID,
        /// Value : Transaction
        /// </summary>
        private volatile Dictionary<Guid, WebScrapingContext> _activeContexts;

        #region Singleton
        private static volatile DatabaseTransactionQueue _instance;


        public static DatabaseTransactionQueue Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new DatabaseTransactionQueue();

                return _instance;
            }
        }
        #endregion

        public DatabaseTransactionQueue() : base()
        {
            if (this._activeContexts == null)
                this._activeContexts = new Dictionary<Guid, WebScrapingContext>();

            this.Queue = new PriorityQueue<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>>
                (Configuration.Instance.Queues.DatabaseTransactionQueue.Capacity, 
                new DatabaseOperationComparer());
        }


        /// <summary>
        /// Creates a database context to perform changes on. Every operation get a different instance of WebScrapingContext
        /// </summary>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns>Returns a unique WebScrapingContext instance to perform database changes.</returns>
        public WebScrapingContext GetContext(DatabaseProcessKey key, DbContextOptions<WebScrapingContext> options = null)
        {
            if (this._activeContexts.ContainsKey(key.OperationID) == false)
                this.InitContext(key.OperationID, options);
            return this._activeContexts[key.OperationID];
        }

        public void DisposeContext(DatabaseProcessKey key)
        {
            this._activeContexts[key.OperationID].Dispose();
            this._activeContexts.Remove(key.OperationID);
        }

        /// <summary>
        /// Queues a database transaction to be performed in given priority.
        /// </summary>
        /// <param name="key">DatabaseProcessKey.GenerateKey with a desired priority</param>
        /// <param name="transaction">EF database transaction</param>
        public override void Enqueue(KeyValuePair<DatabaseProcessKey, IDbContextTransaction> item)
        {
            if (this._activeContexts.TryGetValue(item.Key.OperationID, out WebScrapingContext context) == false)
                throw new Exception("No active context could be found for this thread. Please define database context before queuing a transaction");

            //Save changes before enqueue
            context.Database.AutoTransactionsEnabled = false;
            context.SaveChanges();

            //Ensure the transaction queue is running.
            base.Enqueue(item);
        }

        public void Enqueue(DatabaseProcessKey databaseProcessKey, IDbContextTransaction transaction)
        {
            this.Enqueue(new KeyValuePair<DatabaseProcessKey, IDbContextTransaction>(databaseProcessKey, transaction));
        }

        private void InitContext(Guid operationID, DbContextOptions<WebScrapingContext> options = null)
        {
            WebScrapingContext newContext = options == null ? new WebScrapingContext() : new WebScrapingContext(options);
            newContext.Database.AutoTransactionsEnabled = false;
            this._activeContexts.Add(operationID, newContext);
        }


        /// <summary>
        /// Enqueues the next item from the priority queue and processes it.
        /// </summary>
        /// <returns>Boolean success</returns>
        protected override async Task<Object> ProcessItem(KeyValuePair<DatabaseProcessKey, IDbContextTransaction> process)
        {
            //Lock the thread so only one item would be processed at a time.

            if (this._activeContexts.TryGetValue(process.Key.OperationID, out WebScrapingContext context) == false)
            {
                this.WriteLog(new Exception(String.Format("Could not find any active context for thread '{0}'", process.Key.OperationID.ToString())));
                this.WriteLog("warning", Environment.StackTrace);
                return false;
            }

            using (context)
            using (IDbContextTransaction transaction = process.Value)
            {
                await transaction.CreateSavepointAsync("Before");
                await transaction.CommitAsync();
            }

            return true;

        }

        /// <summary>
        /// Dumps database context belonging to the thread. This should be called from Thread.Stop method.
        /// </summary>
        /// <param name="item"></param>
        protected override void DisposeItem(KeyValuePair<DatabaseProcessKey, IDbContextTransaction> item)
        {
            this._activeContexts[item.Key.OperationID].DisposeAsync();
            this._activeContexts.Remove(item.Key.OperationID);
        }

        protected override void OnException(Exception ex, KeyValuePair<DatabaseProcessKey, IDbContextTransaction> item)
        {
            item.Value.RollbackToSavepointAsync("Before");
        }
    }

    public class DatabaseOperationComparer : IComparer<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>>
    {
        public int Compare(KeyValuePair<DatabaseProcessKey, IDbContextTransaction> p1, KeyValuePair<DatabaseProcessKey, IDbContextTransaction> p2)
        {
            int x = (Int32)p1.Key.Priority;
            int y = (Int32)p2.Key.Priority;

            if (x == y)
                return 0;
            else if (x > y)
                return -1;
            else
                return 1;
        }
    }
}
