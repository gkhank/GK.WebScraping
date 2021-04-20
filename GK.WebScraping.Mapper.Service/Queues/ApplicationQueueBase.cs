using GK.WebScraping.Model.Code.Collections.GK.WebScraping.Model.Collection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GK.WebScraping.Mapper.Service.Queues

{
    public abstract class ApplicationQueueBase<T>
    {
        protected static readonly object _lock = new object();
        protected static readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1);
        protected volatile PriorityQueue<T> Queue;

        public event EventHandler ThresholdReached;
        public delegate void ThresholdReachedEventHandler(object sender, EventArgs e);

        protected ILogger _logger;
        public virtual Boolean RetryFailedOperations => true;
        public virtual Int32 NumberOfMaximumThreads => 1;
        protected int NumberOfThreads = 0;

        public Boolean IsRunning { get { return this.NumberOfThreads > 0; } }

        private Dictionary<T, Action<T, Object>> _callBackCollection;

        protected Dictionary<T, Action<T, Object>> CallbackCollection
        {
            get
            {
                if (this._callBackCollection == null)
                    this._callBackCollection = new Dictionary<T, Action<T, Object>>();
                return this._callBackCollection;
            }
        }

        protected ApplicationQueueBase()
        {

        }

        /// <summary>
        /// Use this to initiate a global logged to receive log messages from DatabaseTransactionQueueManager
        /// </summary>
        /// <param name="logger"></param>
        /// 

        #region Logger methods
        public void InitLogger(ILogger logger)
        {
            this._logger = logger;
        }

        protected void WriteLog(String type, String format, params Object[] args)
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

        protected void WriteLog(Exception ex)
        {
            if (this._logger != null)
                this._logger.LogError(ex, "{0} \n {1}", ex.Message, ex.StackTrace);
        }

        #endregion


        /// <summary>
        /// Ensures that process queue is running.
        /// </summary>
        protected virtual void StartProcessingQueue()
        {

            // Offload this to a background thread (so that the UI is not affected)
            var queueProcessingTask = Task.Run(() =>
            {
                this.NumberOfThreads++;
                while (this.Queue.Count > 0)
                {

                    // Get the next item
                    var nextItem = this.Queue.Peek();

                    // Try to process one item...
                    var isSuccessful = this.InnerProcess(nextItem).GetAwaiter().GetResult();

                    // If we processed successfully or if we don't want to retry it, then we can dequeue the item
                    if (isSuccessful || this.RetryFailedOperations == false)
                    {
                        this.Queue.Dequeue();
                        this.DisposeItem(nextItem);
                        Debug.WriteLine($"Items in '{this}' queue : {this.Queue.Count}");
                    }
                }
                this.NumberOfThreads--;
            });

        }


        private async Task<Boolean> InnerProcess(T item)
        {
            await _asyncLock.WaitAsync();

            Boolean isSuccessful = false;
            try
            {
                if (this.Queue.Capacity <= this.Queue.Count &&
                    this.ThresholdReached != null)
                {
                    this.ThresholdReached.Invoke(this, EventArgs.Empty);
                }

                var retval = await this.ProcessItem(item);

                isSuccessful = true;

                if (this.CallbackCollection.TryGetValue(item, out Action<T, Object> callback))
                    this.DoCallback(callback, item, retval);

            }
            catch (Exception ex)
            {
                isSuccessful = false;
                this.WriteLog(ex);
                this.OnException(ex, item);
            }
            finally
            {
                _asyncLock.Release();
            }

            return isSuccessful;

        }


        protected abstract Task<Object> ProcessItem(T process);

        public virtual void Enqueue(T item)
        {
            this.Queue.Enqueue(item);

            if (!this.IsRunning)
                this.StartProcessingQueue();
        }

        public virtual void Enqueue(T item, Action<T, Object> callback)
        {
            this.CallbackCollection.Add(item, callback);
            this.Queue.Enqueue(item);

            if (!this.IsRunning)
                this.StartProcessingQueue();

        }
        private void DoCallback(Action<T, Object> callback, T sender, object retval)
        {
            //This supports a single parameter callbacks that will act as a return value;
            callback.Invoke(sender, retval);
        }

        protected abstract void DisposeItem(T item);
        protected abstract void OnException(Exception ex, T item);

    }
}
