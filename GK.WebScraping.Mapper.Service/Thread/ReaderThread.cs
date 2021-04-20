using GK.WebScraping.DB;
using GK.WebScraping.Model;
using GK.WebScraping.Model.Code.Operations;
using GK.WebScraping.Utilities;
using GK.WebScraping.Utilities.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            DatabaseTransactionQueue.Instance.InitLogger(logger);
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


                DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.Low);
                this._context = DatabaseTransactionQueue.Instance.GetContext(operationKey);
                IDbContextTransaction transaction = this._context.Database.BeginTransaction();

                Dictionary<Int32, Page> pages = this.GetPagesData();

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

                List<String> links = new List<string>();
                try
                {
                    foreach (var page in pages.Values)
                    {
                        String html = null;
                        this.Update(page, ref html, ref links);
                        processCount++;
                    }

                    //Mark as finished so it can be picked up by any other thread.
                    OperationCollection<Int32>.MarkAsCompleted(pages.Keys);
                    OperationCollection<String>.MarkAsCompleted(links);
                    DatabaseTransactionQueue.Instance.Enqueue(operationKey, transaction);

                }
                catch (Exception e)
                {
                    this._logger.LogError(e, e.Message);
                }
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

        private Dictionary<Int32, Page> GetPagesData()
        {

            try
            {
                DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.Low);

                using (var innerContext = DatabaseTransactionQueue.Instance.GetContext(operationKey))
                {
                    Dictionary<Int32, Page> retval = innerContext.Pages
                        .Where(x =>
                        OperationCollection<Int32>.OngoingProcesses.Contains(x.PageId) == false &&
                        OperationCollection<String>.OngoingProcesses.Contains(x.Url) == false &&
                        x.DeleteDate.HasValue == false &&
                        x.Status == (Int32)StatusType.Active &&
                        (x.LastReadDate.HasValue == false || x.LastReadDate <= this._lastAcceptableReadDate))
                        .Take(this._bulkSize)
                        .Include(x => x.Store)
                        .ToDictionary(x => x.PageId);

                    OperationCollection<Int32>.MarkAsOngoing(retval.Keys);
                    return retval;
                }


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

            foreach (String pageUrl in pageUrls)
            {
                if (OperationCollection<String>.OngoingProcesses.Contains(pageUrl) == false)
                {
                    OperationCollection<String>.OngoingProcesses.Add(pageUrl);

                    Page newPage = new Page()
                    {
                        LastReadDate = null,
                        CreateDate = DateTime.Now,
                        DeleteDate = null,
                        MapStatus = (short)MapStatusType.None,
                        Url = pageUrl,
                        Status = (short)StatusType.Active,
                        StoreId = storeId //Assign same storeId from parent page.
                    };

                    this._context.Pages.Update(newPage);
                    retval.Add(pageUrl);
                }
            }


            return retval.ToArray();

        }

        private void Update(Page page, ref string html, ref List<string> links)
        {
            try
            {
                //Update content
                String fileName = page.PageId + ".html";
                String directoryPath = Path.Combine(FileHelper.PagesDirectory, page.StoreId.ToString());

                if (Directory.Exists(directoryPath) == false)
                    Directory.CreateDirectory(directoryPath);

                String fullPath = Path.Combine(directoryPath, fileName);

                if (File.Exists(fullPath))
                {
                    DateTime lastFileUpdateTime = new FileInfo(fullPath).LastWriteTime;
                    if (lastFileUpdateTime < this._lastAcceptableReadDate)
                    {
                        this.UpdateFileFromRemote(page.StoreId, page.Store.RootUrl, page.Url, fullPath, ref html, ref links);
                        this.UpdateDatabaseContext(page, DateTime.Now);
                    }
                    else
                    {
                        FileOperation fileOperation = FileOperation.Create(fullPath, FileOperation.OperationType.Read);
                        fileOperation.Metadata.Add("Page", page);
                        FileOperationsQueue.Instance.Enqueue(fileOperation, this.FileReadingCompleted);
                    }
                }
                else
                {
                    this.UpdateFileFromRemote(page.StoreId, page.Store.RootUrl, page.Url, fullPath, ref html, ref links);
                    this.UpdateDatabaseContext(page, DateTime.Now);
                }
            }
            catch (IOException iex)
            {
                this._logger.LogError(iex, iex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
            }
        }

        private void FileReadingCompleted(object sender, object processResult)
        {
            String html = processResult as String;
            FileOperation operation = sender as FileOperation;

            if (operation.Metadata.TryGetValue("Page", out object pageObject))
            {
                Page page = pageObject as Page;

                OperationCollection<Int32>.OngoingProcesses.Add(page.PageId);

                page.LastReadDate = DateTime.Now;

                DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.High);
                var context = DatabaseTransactionQueue.Instance.GetContext(operationKey);

                IDbContextTransaction transaction = context.Database.BeginTransaction();
                context.Update(page);

                DatabaseTransactionQueue.Instance.Enqueue(operationKey, transaction);
            }
        }

        private void UpdateFileFromRemote(Guid storeID, String rootUrl, String url, String fullPath, ref String html, ref List<string> links)
        {
            html = this._htmlUtils.GetHtmlContent(url);

            var htmlLinks = this._htmlUtils.GetLinksInHtml(html, rootUrl);

            links.AddRange(htmlLinks);

            this.InsertPageLinks(storeID, htmlLinks.ToArray());

            FileOperation operation = FileOperation.Create(fullPath, FileOperation.OperationType.CreateOrUpdate, html);
            FileOperationsQueue.Instance.Enqueue(operation);
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
