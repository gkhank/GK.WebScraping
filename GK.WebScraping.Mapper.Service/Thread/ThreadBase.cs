using GK.WebScraping.DB;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GK.WebScraping.Mapper.Service.Thread
{
    public abstract class ThreadBase
    {
        protected System.Threading.Thread _thread;
        protected object _lock = new object();
        private Stopwatch _overallWatch;
        private Stopwatch _processWatch;
        protected ILogger _logger;
        protected abstract String ThreadName { get; set; }
        protected ReaderWriterLockSlim _rwLock;


        public ThreadBase(ILogger logger)
        {
            this._overallWatch = new Stopwatch();
            this._processWatch = new Stopwatch();
            this._logger = logger;
            this._lock = new object();
            this._rwLock = new ReaderWriterLockSlim();
        }

        public void Start()
        {
            this._thread = new System.Threading.Thread(InnerProcess) { IsBackground = true };
            this._overallWatch.Start();
            this._thread.Start();
            this._logger.LogInformation("{0} thread started.", this.ThreadName);

        }

        private void InnerProcess(object obj)
        {
            this._logger.LogInformation("{0} iteration started.", this.ThreadName);

            this._processWatch.Start();
            this.Process();
            this._processWatch.Stop();
            this._logger.LogInformation("Iteration ended in thread {0} and completed in {1} ms", this.ThreadName, this._processWatch.ElapsedMilliseconds);
        }

        protected abstract void Process();

        public virtual void Stop()
        {
            this._overallWatch.Stop();
            this._logger.LogInformation("{0} is stopped and it was running for {1} seconds", this.ThreadName, this._overallWatch.ElapsedMilliseconds / 1000);
            if (this._thread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                this._thread.Interrupt();
            }
        }

        public Int32 SaveDatabaseChanges()
        {
            if (DatabaseManager.WebScraping.ChangeTracker.HasChanges())
            {
                this._rwLock.EnterWriteLock();
                Int32 count = DatabaseManager.WebScraping.SaveChanges();
                this._rwLock.ExitWriteLock();
                this._logger.LogInformation("Operation updated {0} new pages", count);
                return count;
            }
            return 0;

        }
    }
}
