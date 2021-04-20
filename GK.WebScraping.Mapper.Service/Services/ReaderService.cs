using System;
using System.Collections.Generic;
using GK.WebScraping.Mapper.Service.Queues;
using GK.WebScraping.Mapper.Service.Thread;
using GK.WebScraping.Utilities;
using Microsoft.Extensions.Logging;

namespace GK.WebScraping.Mapper.Service
{
    public class ReaderService : ServiceBase
    {
        List<ThreadBase> _threads;
        public ReaderService(ILogger<ReaderService> logger) : base(logger)
        {
            DatabaseTransactionQueue.Instance.ThresholdReached += TresholdReached;
            FileOperationsQueue.Instance.ThresholdReached += TresholdReached;
        }

        private void TresholdReached(object sender, EventArgs e)
        {
            this._logger.LogWarning(sender.ToString() + " reached max capacity. Sleeping threads for 5 minutes");

            for (int i = 0; i < this._threads.Count; i++)
                this._threads[i].SleepThread(DateTime.Now.AddMinutes(5));
        }

        protected override void Init()
        {
            this._threads = new List<ThreadBase>();

            for (int i = 0; i < Configuration.Instance.Services.ReaderService.NumberOfThreads; i++)
                this._threads.Add(ReaderThread.Create(this._logger, i, Configuration.Instance.Services.ReaderService.BulkSize));
        }

        protected override void Start()
        {
            for (int i = 0; i < this._threads.Count; i++)
            {
                System.Threading.Thread.Sleep(5000);
                this._threads[i].Start();
            }
        }

        protected override void Stop()
        {
            for (int i = 0; i < this._threads.Count; i++)
                this._threads[i].Stop();

            this._threads.Clear();
            this.Dispose();
        }
    }
}
