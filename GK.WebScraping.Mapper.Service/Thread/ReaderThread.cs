using GK.WebScraping.DB;
using GK.WebScraping.Model;
using GK.WebScraping.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GK.WebScraping.Mapper.Service.Thread
{
    public class ReaderThread : ThreadBase
    {
        private readonly HtmlUtilities _htmlUtils;
        private readonly int _bulkSize;
        private DateTime _lastAcceptableReadDate;
        private Timer _sleepTimer;

        protected override string ThreadName { get; set; }
        public ReaderThread(ILogger logger, Int16 index, Int32 bulkSize) : base(logger)
        {
            this.ThreadName = String.Format("{0}.{1}", this.ToString(), index);
            this._htmlUtils = new HtmlUtilities();
            this._bulkSize = bulkSize;
        }

        private void TimerCallBack(object state)
        {

            //Restart the thread
            if (this._thread.IsAlive == false)
                this.Start();
        }

        protected override void Process()
        {
            this._logger.LogInformation("{0} started at: {time}", this.ThreadName, DateTimeOffset.Now);

            Dictionary<Guid, Store> storesById = DatabaseManager.WebScraping.Stores.ToDictionary(x => x.StoreId);

            while (true)
            {
                //Prepare links for read operation. Exclude pages that we previously read.
                this._lastAcceptableReadDate = DateTime.Now.AddDays(-1);


                var pages = DatabaseManager.WebScraping.Pages
                    .Where(x =>
                    OperationCollection<Guid>.OngoingProcesses.Contains(x.PageId) == false &&
                    OperationCollection<String>.OngoingProcesses.Contains(x.Url) == false &&
                    x.DeleteDate.HasValue == false &&
                    x.Status == (Int32)StatusType.Active &&
                    (x.LastReadDate.HasValue == false || x.LastReadDate <= this._lastAcceptableReadDate)).Take(this._bulkSize).ToDictionary(x => x.PageId);


                //If there is no pages to read. Sleep thread until next time...
                if (pages.Count == 0)
                {
                    //DateTime sleepUntil = DateTime.Now.AddSeconds(30);
                    DateTime sleepUntil = DatabaseManager.WebScraping.Pages.Min(x => x.LastReadDate).Value.AddDays(1);
                    TimeSpan span = sleepUntil - DateTime.Now;
                    this._logger.LogInformation("Sleeping thread until next execution time '{0}'", sleepUntil.ToString("yyyy-MM-dd HH:mm:ss"));
                    this._sleepTimer = new Timer(this.TimerCallBack, this, span, span);
                    this.Stop();
                    return;
                }



                //A dd result to ongoing processes to prevent other threads to process same items.
                Guid[] pageIdsArray = pages.Keys.ToArray();
                OperationCollection<Guid>.MarkAsOngoing(pageIdsArray);

                Int32 processCount = 0;

                //Read contents for given url set.
                foreach (var page in pages.Values)
                {
                    String html = null;

                    this.Update(page, ref html);
                    processCount++;

                    if (storesById.TryGetValue(page.StoreId, out Store store) == false)
                    {
                        this._logger.LogWarning("Could not relate any store with storeId '{0}'", page.StoreId);
                        continue;
                    }

                    //Read all links for every content and save them to database to be read on next execution.
                    String[] pageUrls = _htmlUtils.GetLinksInHtml(html, store.RootUrl).ToArray();
                    OperationCollection<String>.MarkAsOngoing(pageUrls);
                    this.SavePageLinks(page.StoreId, pageUrls);

                    this.SaveDatabaseChanges();

                    //Mark as finished so it can be picked up by any other thread.
                    OperationCollection<Guid>.MarkAsCompleted(page.PageId);
                    OperationCollection<String>.MarkAsCompleted(pageUrls);
                }

                this._logger.LogInformation("Iteration finished and {0} processed {1} pages.", this.ThreadName, processCount);
            }
        }

        private void SavePageLinks(Guid storeId, params string[] pageUrls)
        {
            if (pageUrls.Length <= 0)
                return;


            Dictionary<String, Page> databasePages = DatabaseManager.WebScraping.Pages.Where(x => pageUrls.Contains(x.Url)).ToDictionary(x => x.Url);

            foreach (String pageUrl in pageUrls)
            {
                //Not in database.
                if (databasePages.TryGetValue(pageUrl, out Page existingPage) == false &&
                    OperationCollection<String>.OngoingProcesses.Contains(pageUrl) == false)
                {

                    Page newPage = new Page()
                    {
                        PageId = Guid.NewGuid(),
                        LastReadDate = null,
                        CreateDate = DateTime.Now,
                        DeleteDate = null,
                        MapStatus = (short)MapStatusType.None,
                        Url = pageUrl,
                        Status = (short)StatusType.Active,
                        StoreId = storeId //Assign same storeId from parent page.
                    };

                    DatabaseManager.WebScraping.Pages.Add(newPage);

                }
            }

        }

        private void Update(Page page, ref string html)
        {
            try
            {
                //Update content
                String fileName = String.Format("{0}.html", page.PageId.ToString()); // DateTime.Now.ToString("yyyy_MM_dd_HH_mm")
                String directoryPath = Path.Combine(FileHelper.PagesDirectory, page.StoreId.ToString());

                if (Directory.Exists(directoryPath) == false)
                    Directory.CreateDirectory(directoryPath);

                String fullPath = Path.Combine(directoryPath, fileName);

                if (File.Exists(fullPath))
                {
                    DateTime lastFileUpdateTime = new FileInfo(fullPath).LastWriteTime;
                    if (lastFileUpdateTime < this._lastAcceptableReadDate)
                    {
                        this.UpdateFile(page.Url, fullPath, ref html);
                        this.UpdateDatabase(page, DateTime.Now);
                    }
                    else
                    {
                        this._logger.LogWarning("File was saved at '{0}'. Attempted to read HTML unneccsarily for '{0}'. Using existing content.", lastFileUpdateTime.ToString("yyyy-MM-dd HH:mm"), page.Url);
                        //Save read time on database so we won't try anymore...
                        this.UpdateDatabase(page, lastFileUpdateTime);
                        html = this.ReadFile(fullPath);
                    }
                }
                else
                {
                    this.UpdateFile(page.Url, fullPath, ref html);
                    this.UpdateDatabase(page, DateTime.Now);

                }


            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
            }
        }

        private string ReadFile(string fullPath)
        {
            return File.ReadAllText(fullPath);
        }

        private void UpdateFile(String url, String fullPath, ref String html)
        {

            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    Stopwatch readWatch = new Stopwatch();
                    readWatch.Start();
                    this._logger.LogInformation("Reading content for '{0}'...", url);
                    html = _htmlUtils.GetHtmlContent(url);
                    writer.Write(html);
                    readWatch.Stop();
                    this._logger.LogInformation("Reading content is done and process was done in {0} ms.", readWatch.ElapsedMilliseconds);

                    writer.Dispose();
                }
                fs.Dispose();
            }

        }

        private void UpdateDatabase(Page page, DateTime updateDate)
        {
            page.MapStatus = (short)MapStatusType.ContentReady;
            page.LastReadDate = updateDate;
        }

        public override void Stop()
        {
            OperationCollection<Guid>.DisposeCompleted();
            OperationCollection<String>.DisposeCompleted();

            base.Stop();
        }
    }
}
