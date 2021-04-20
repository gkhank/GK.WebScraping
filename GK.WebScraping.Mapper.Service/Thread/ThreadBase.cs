using GK.WebScraping.DB;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;

namespace GK.WebScraping.Mapper.Service.Thread
{
    public abstract class ThreadBase
    {
        protected System.Threading.Thread _thread;
        protected object _lock = new object();
        protected Timer _sleepTimer;
        protected ILogger _logger;
        protected bool _run;

        private Stopwatch _overallWatch;
        private Stopwatch _processWatch;

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

        public void SleepThread(DateTime sleepUntil)
        {
            TimeSpan span = sleepUntil - DateTime.Now;
            this._logger.LogInformation("Sleeping thread until next execution time '{0}'", sleepUntil.ToString("yyyy-MM-dd HH:mm:ss"));
            this._sleepTimer = new Timer(this.TimerCallBack, this, span, span);
            this.Stop();
        }

        private void TimerCallBack(object state)
        {

            //Restart the thread
            if (this._run == false)
            {
                this._run = true;
                this.Start();
            }
        }

        protected abstract void Process();

        public virtual void Stop()
        {
            this._overallWatch.Stop();
            this._logger.LogInformation("{0} is stopped and it was running for {1} seconds", this.ThreadName, this._overallWatch.ElapsedMilliseconds / 1000);
        }
    }
}
