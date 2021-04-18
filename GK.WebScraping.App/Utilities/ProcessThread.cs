using GK.WebScraping.Model;
using GK.WebScraping.Utilities;
using System;
using System.Threading;

namespace GK.WebScraping.App.Utilities
{
    public class ProcessThread
    {
        private System.Threading.Thread _thread;
        private readonly DateTime _stopTime;
        private SearchOptions _options;
        private Form1.UpdateEventHandler _updateCallbackEvent { get; }
        private static readonly object _lock = new object();
        private Int32 _runCount = 0;

        public ProcessThread( SearchOptions options, Form1.UpdateEventHandler updateCallbackEvent)
        {
            this._stopTime = DateTime.Now.AddMinutes(options.RunHours);
            this._options = options;
            this._updateCallbackEvent = updateCallbackEvent;
        }

        public void Start()
        {
            this._thread = new System.Threading.Thread(Process) { IsBackground = true };
            this._thread.Start();
        }
        private void Process()
        {

            this._updateCallbackEvent.Invoke(this, ProcessUpdateType.StartingThread);

            while ((this._options.RunCount.HasValue == false && DateTime.Now < this._stopTime)
                    || this._options.RunCount > 0)
            {

                lock (_lock)
                {
                    this._runCount++;

                    this._updateCallbackEvent.Invoke(this, ProcessUpdateType.LoopStarting, this._runCount);

                    foreach (var item in this._options.SelectedStores)
                    {
                        Guid storeId = Guid.Parse(item.ToString());


                        //if (this._manager.TryGetStore(Store, out IStore store))
                        //{
                        //    try
                        //    {
                        //        ConsoleAgent.Write("Searching for {0} in {1} store...", color: "white", bgcolor: "", doSpeak: false, this._options.Keyword, storeName);
                        //        GenericResponse response = store.GetResponse(this._options);

                        //        foreach (GenericProduct p in response.Products)
                        //        {
                        //            this.UpdateCallbackEvent.Invoke(this, ProcessUpdateType.Found, p.ProductID, p.Name, p.UnitsInStock, p.UnitPriceString, p.Url);
                        //        }

                        //        Boolean hasStock = response.Products.Count > 0;
                        //        if (hasStock)
                        //            ConsoleAgent.Write("Found {0} items in {1}.", color: "green", bgcolor: "", doSpeak: true, response.Products.Count, storeName);
                        //        else
                        //            ConsoleAgent.Write("No items found in {0}", color: "yellow", bgcolor: "", doSpeak: false, storeName);

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        this.UpdateCallbackEvent.Invoke(this, ProcessUpdateType.Error, ex);

                        //    }

                        //}
                        //else
                        //    ConsoleAgent.Write(format: "Could not load store {0}.", color: "blue", bgcolor: "black", args: storeName);

                    }

                }

                if (this._options.WaitSeconds > 0)
                {
                    ConsoleAgent.Write(format: "Waiting {0} seconds for searching again", color: "blue", bgcolor: "black", args: this._options.WaitSeconds);
                    System.Threading.Thread.Sleep(_options.WaitSeconds * 1000);
                }


                if (this._options.RunCount.HasValue)
                {
                    this._options.RunCount--;
                    this._updateCallbackEvent.Invoke(this, ProcessUpdateType.UpdateRunCount, this._options.RunCount);
                }

            }

            this._updateCallbackEvent.Invoke(this, ProcessUpdateType.EndingThread);
        }

        public void Update(SearchOptions options)
        {
            this._options = options;
        }

        public void Stop()
        {
            if (this._thread.ThreadState != ThreadState.Aborted)
            {
                this._thread.Abort();
                this._thread = null;
                this._updateCallbackEvent.Invoke(this, ProcessUpdateType.ThreadStopped);

            }
        }

    }
}
