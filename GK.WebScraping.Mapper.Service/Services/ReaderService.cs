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

        protected override void InitThreads()
        {
            this._threads = new List<ThreadBase>();
            this._threads.Add(new ReaderThread(this._logger, 1, 20));
            //this._threads.Add(new ReaderThread(this._logger, 2, 20));
            //this._threads.Add(new ReaderThread(this._logger, 3, 20));
            //this._threads.Add(new ReaderThread(this._logger, 5, 20));
            //this._threads.Add(new ReaderThread(this._logger, 6, 20));
            //this._threads.Add(new ReaderThread(this._logger, 7, 20));
            //this._threads.Add(new ReaderThread(this._logger, 8, 20));
            //this._threads.Add(new ReaderThread(this._logger, 9, 20));
            //this._threads.Add(new ReaderThread(this._logger, 10, 20));
            //this._threads.Add(new ReaderThread(this._logger, 11, 20));
            //this._threads.Add(new ReaderThread(this._logger, 12, 20));
            //this._threads.Add(new ReaderThread(this._logger, 13, 100));
            //this._threads.Add(new ReaderThread(this._logger, 14, 100));
            //this._threads.Add(new ReaderThread(this._logger, 15, 100));
            //this._threads.Add(new ReaderThread(this._logger, 16, 100));
            //this._threads.Add(new ReaderThread(this._logger, 17, 100));
            //this._threads.Add(new ReaderThread(this._logger, 18, 100));
            //this._threads.Add(new ReaderThread(this._logger, 19, 100));
            //this._threads.Add(new ReaderThread(this._logger, 20, 100));
            //this._threads.Add(new ReaderThread(this._logger, 21, 100));
            //this._threads.Add(new ReaderThread(this._logger, 22, 100));
            //this._threads.Add(new ReaderThread(this._logger, 23, 100));
            //this._threads.Add(new ReaderThread(this._logger, 24, 100));
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
