using GK.WebScraping.DB;
using Microsoft.EntityFrameworkCore.Storage;
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


        public ThreadBase(ILogger logger)
        {
            this._overallWatch = new Stopwatch();
            this._processWatch = new Stopwatch();
            this._logger = logger;
            this._lock = new object();
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
        }

        public Int32 SaveDatabaseChanges(WebScrapingContext context)
        {
            if (context.ChangeTracker.HasChanges())
            {

                using (IDbContextTransaction writeTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        writeTransaction.CreateSavepoint("BeforeWrite");
                        Int32 count = context.SaveChanges();
                        this._logger.LogInformation("Operation updated {0} new pages", count);
                        writeTransaction.Commit();
                        return count;

                    }
                    catch (Exception ex)
                    {
                        writeTransaction.RollbackToSavepoint("BeforeWrite");
                        throw ex;
                    }

                }
            }

            return 0;

        }
    }
}
