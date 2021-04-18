using GK.WebScraping.DB;
using GK.WebScraping.Model;
using GK.WebScraping.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        private bool _run;
        private WebScrapingContext _context;

        protected override string ThreadName { get; set; }
        public ReaderThread(ILogger logger, Int16 index, Int32 bulkSize) : base(logger)
        {
            this.ThreadName = String.Format("{0}.{1}", this.ToString(), index);
            this._htmlUtils = new HtmlUtilities();
            this._bulkSize = bulkSize;
            this._run = true;
            DatabaseTransactionQueueManager.Instance.InitLogger(logger);
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

        protected override void Process()
        {
            while (this._run)
            {
                this._lastAcceptableReadDate = DateTime.Now.AddDays(-1);


                DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(DatabaseQueuePriorityType.Normal);
                this._context = DatabaseTransactionQueueManager.Instance.GetContext(operationKey);

                Dictionary<Guid, Page> pages = this.GetPagesData();

                //If there is no pages to read. Sleep thread until next time...
                if (pages == null ||
                    pages.Count == 0)
                {
                    DateTime sleepUntil = this.NextRunTime();
                    TimeSpan span = sleepUntil - DateTime.Now;
                    this._logger.LogInformation("Sleeping thread until next execution time '{0}'", sleepUntil.ToString("yyyy-MM-dd HH:mm:ss"));
                    this._sleepTimer = new Timer(this.TimerCallBack, this, span, span);
                    this.Stop();
                    return;
                }

                Int32 processCount = 0;

                IDbContextTransaction transaction = this._context.Database.BeginTransaction();
                try
                {
                    List<String> processedUrls = new List<string>();
                    foreach (var page in pages.Values)
                    {
                        String html = null;

                        this.Update(page, ref html);
                        processCount++;

                        //Read all links for every content and save them to database to be read on next execution.
                        String[] pageUrls = _htmlUtils.GetLinksInHtml(html, page.Store.RootUrl).ToArray();
                        processedUrls.AddRange(this.InsertPageLinks(page.StoreId, pageUrls));

                    }
                    processCount += processedUrls.Count;

                    //Mark as finished so it can be picked up by any other thread.
                    OperationCollection<Guid>.MarkAsCompleted(pages.Keys);
                    OperationCollection<String>.MarkAsCompleted(processedUrls);


                    DatabaseTransactionQueueManager.Instance.QueueTransaction(operationKey, transaction);

                }
                catch (Exception e)
                {
                    this._logger.LogError(e, e.Message);
                }

                this._logger.LogInformation("Iteration finished and {0} processed {1} pages.", this.ThreadName, processCount);
            }
        }

        private DateTime NextRunTime()
        {
            if (Configurations.IsDevelopment)
                return DateTime.Now.AddMinutes(1);

            using (var context = new WebScrapingContext())
            using (IDbContextTransaction readTransaction = context.Database.BeginTransaction())
            {
                return context.Pages.Min(x => x.LastReadDate).Value.AddDays(1);
            }
        }

        private Dictionary<Guid, Page> GetPagesData()
        {

            try
            {
                Dictionary<Guid, Page> retval = this._context.Pages
                    .Where(x =>
                    OperationCollection<Guid>.OngoingProcesses.Contains(x.PageId) == false &&
                    OperationCollection<String>.OngoingProcesses.Contains(x.Url) == false &&
                    x.DeleteDate.HasValue == false &&
                    x.Status == (Int32)StatusType.Active &&
                    (x.LastReadDate.HasValue == false || x.LastReadDate <= this._lastAcceptableReadDate))
                    .Take(this._bulkSize)
                    .Include(x => x.Store)
                    .ToDictionary(x => x.PageId);


                OperationCollection<Guid>.MarkAsOngoing(retval.Keys);

                return retval;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private string[] InsertPageLinks(Guid storeId, params string[] pageUrls)
        {
            if (pageUrls.Length <= 0)
                return pageUrls;


            List<String> retval = new List<string>();
            Dictionary<String, Page> databasePages = new Dictionary<string, Page>();
            var existingPages = this._context.Pages.Where(x => pageUrls.Contains(x.Url));
            foreach (var ep in existingPages)
            {
                if (databasePages.ContainsKey(ep.Url) == false)
                    databasePages.Add(ep.Url, ep);
            }

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

                    this._context.Pages.Add(newPage);
                    retval.Add(pageUrl);
                    //Lock this url
                    OperationCollection<String>.OngoingProcesses.Add(pageUrl);
                }
            }


            return retval.ToArray();

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
                        this.UpdateDatabaseContext(page, DateTime.Now);
                    }
                    else
                    {
                        this._logger.LogWarning("File was saved at '{0}'. Attempted to read HTML unneccsarily for '{0}'. Using existing content.", lastFileUpdateTime.ToString("yyyy-MM-dd HH:mm"), page.Url);
                        //Save read time on database so we won't try anymore...
                        this.UpdateDatabaseContext(page, lastFileUpdateTime);
                        html = this.ReadFile(fullPath);
                    }
                }
                else
                {
                    this.UpdateFile(page.Url, fullPath, ref html);
                    this.UpdateDatabaseContext(page, DateTime.Now);

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

        private void UpdateDatabaseContext(Page page, DateTime updateDate)
        {
            page.MapStatus = (short)MapStatusType.ContentReady;
            page.LastReadDate = updateDate;
            this._context.Update(page);

        }

        public override void Stop()
        {
            this._run = false;
            base.Stop();
        }
    }
}
