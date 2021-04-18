using System;
using System.Collections.Generic;
using GK.WebScraping.Mapper.Service.Thread;
using Microsoft.Extensions.Logging;

namespace GK.WebScraping.Mapper.Service
{
    public class ReaderService : ServiceBase
    {
        List<ThreadBase> _threads;
        public ReaderService(ILogger<ReaderService> logger) : base(logger)
        {
        }

        protected override void InitServices()
        {
            this._threads = new List<ThreadBase>();
            this._threads.Add(new ReaderThread(this._logger, 1, 100));
            this._threads.Add(new ReaderThread(this._logger, 2, 100));
            this._threads.Add(new ReaderThread(this._logger, 3, 100));
            this._threads.Add(new ReaderThread(this._logger, 4, 100));
            this._threads.Add(new ReaderThread(this._logger, 5, 100));
            this._threads.Add(new ReaderThread(this._logger, 6, 100));
        }

        protected override void Start()
        {
            for (int i = 0; i < this._threads.Count; i++)
            {
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
