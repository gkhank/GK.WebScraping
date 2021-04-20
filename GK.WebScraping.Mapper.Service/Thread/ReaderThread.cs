using GK.WebScraping.DB;
using GK.WebScraping.Mapper.Service.Queues;
using GK.WebScraping.Model;
using GK.WebScraping.Model.Code.Operations;
using GK.WebScraping.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace GK.WebScraping.Mapper.Service.Thread
{
    public class ReaderThread : ThreadBase
    {
        private readonly HtmlUtilities _htmlUtils;
        private readonly int _bulkSize;
        private DateTime _lastAcceptableReadDate;
        private WebScrapingContext _context;

        protected override string ThreadName { get; set; }
        private ReaderThread(ILogger logger, Int32 index, Int32 bulkSize) : base(logger)
        {
            this.ThreadName = String.Format("{0}.{1}", this.ToString(), index);
            this._htmlUtils = new HtmlUtilities();
            this._bulkSize = bulkSize;
            this._run = true;
            DatabaseTransactionQueue.Instance.InitLogger(logger);
        }

        public static ReaderThread Create(ILogger logger, Int32 index, Int32 bulkSize)
        {
            return new ReaderThread(logger, index, bulkSize);
        }

        protected override void Process()
        {
            while (this._run)
            {
                //Sleep every iteration
                System.Threading.Thread.Sleep(Configuration.Instance.Services.ReaderService.IterationSleep);

                this._lastAcceptableReadDate = DateTime.Now.AddDays(-1);


                DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.Low);
                this._context = DatabaseTransactionQueue.Instance.GetContext(operationKey);
                IDbContextTransaction transaction = this._context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                Dictionary<Int32, Page> pages = this.GetNotProcessedPages();

                //If there is no pages to read. Sleep thread until next time...
                if (pages == null ||
                    pages.Count == 0)
                {
                    DateTime sleepUntil = this.NextRunTime();
                    this.SleepThread(sleepUntil);
                }

                Int32 processCount = 0;

                try
                {
                    foreach (var page in pages.Values)
                    {
                        this.Update(page);
                        processCount++;
                    }

                    DatabaseTransactionQueue.Instance.Enqueue(operationKey, transaction);

                }
                catch (Exception e)
                {
                    this._logger.LogError(e, e.Message);
                }
            }
        }

        public override void Stop()
        {
            this._run = false;
            base.Stop();
        }

        private DateTime NextRunTime()
        {
            if (Configuration.Instance.IsDevelopment)
                return DateTime.Now.AddMinutes(1);

            return this._context.Pages.Min(x => x.LastReadDate).Value.AddDays(1);
        }
        private Dictionary<Int32, Page> GetNotProcessedPages()
        {

            Boolean lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);

                if (lockTaken)
                {
                    String dateString = this._lastAcceptableReadDate.ToString("yyyy-MM-dd HH:mm:ss");
                    Dictionary<Int32, Page> retval = this._context.Pages
                        .Where(
                        x => x.DeleteDate.HasValue == false &&
                        x.Status == (short)StatusType.Active &&
                        (x.LastReadDate.HasValue == false || x.LastReadDate.Value < this._lastAcceptableReadDate))
                        .Include(x => x.Store)
                        .ToDictionary(x => x.PageId);

                    #region Raw sql solution
                    //this._context.Pages.FromSqlRaw($@"
                    //    SELECT TOP {this._bulkSize}
                    //     * 
                    //    FROM 
                    //     [Page]
                    //     WITH (READPAST)
                    //    WHERE
                    //     [Page].status = 1
                    //     AND
                    //     [Page].deleteDate IS NULL
                    //     AND
                    //     (
                    //      [Page].lastReadDate IS NULL
                    //      OR
                    //      [Page].lastReadDate < '{dateString}'
                    //     )
                    //    ")
                    #endregion

                    return retval;


                }
                else
                    throw new Exception("Could not retrieve data within given time");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_lock);
                }
            }

        }
        private Dictionary<String, Page> GetDuplicatePages(WebScrapingContext context, HashSet<String> pageUrls)
        {

            Boolean lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);

                if (lockTaken)
                {
                    var retval = context.Pages.Where(x => pageUrls.Contains(x.Url))
                        .ToDictionary(x => x.Url);

                    return retval;
                }
                else
                    throw new Exception("Could not retrieve data within given time");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_lock);
                }
            }

        }
        private void InsertPageLinks(WebScrapingContext context, Guid storeId, HashSet<string> pageUrls)
        {
            if (pageUrls.Count <= 0)
                return;

            var dbPagesByUrl = this.GetDuplicatePages(context, pageUrls);


            foreach (String pageUrl in pageUrls)
            {
                if (dbPagesByUrl.TryGetValue(pageUrl, out Page page) == false)
                {
                    page = new Page()
                    {
                        LastReadDate = null,
                        CreateDate = DateTime.Now,
                        DeleteDate = null,
                        MapStatus = (short)MapStatusType.ContentReady,
                        Url = pageUrl,
                        Status = (short)StatusType.Active,
                        StoreId = storeId //Assign same storeId from parent page.
                    };
                }

                context.Pages.Update(page);
            }

        }
        private void Update(Page page)
        {
            try
            {
                //Update content
                String fileName = page.PageId + ".html";
                String directoryPath = Path.Combine(ApplicationPath.PagesDirectory, page.StoreId.ToString());

                if (Directory.Exists(directoryPath) == false)
                    Directory.CreateDirectory(directoryPath);

                String fullPath = Path.Combine(directoryPath, fileName);

                //File exists
                if (File.Exists(fullPath))
                {
                    DateTime lastFileUpdateTime = new FileInfo(fullPath).LastWriteTime;

                    //File exists but not valid. Read it again from remote.
                    if (lastFileUpdateTime < this._lastAcceptableReadDate)
                    {
                        this.UpdateFileFromRemote(page.StoreId, page.Store.RootUrl, page.Url, fullPath);
                        this.UpdateReadDate(page, DateTime.Now);
                    }
                    //File exists and still valid. Add it to read queue to be processed.
                    else
                    {
                        FileOperation fileOperation = FileOperation.Create(fullPath, FileOperation.OperationType.Read);
                        fileOperation.Metadata.Add("Page", page);
                        FileOperationsQueue.Instance.Enqueue(fileOperation, this.FileReadingCompleted);
                    }
                }
                //File does not exists.
                else
                {
                    this.UpdateFileFromRemote(page.StoreId, page.Store.RootUrl, page.Url, fullPath);
                    this.UpdateReadDate(page, DateTime.Now);
                }
            }
            catch (IOException iex)
            {
                this._logger.LogError(iex, iex.Message);
            }
            catch (WebException wex)
            {
                if (wex.Message != "The operation has timed out.")
                    this._logger.LogError(wex, wex.Message);
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
                page.LastReadDate = DateTime.Now;
                page.MapStatus = (short)MapStatusType.ContentReady;

                DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.Normal);
                WebScrapingContext context = DatabaseTransactionQueue.Instance.GetContext(operationKey);

                IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                context.Update(page);

                HashSet<String> htmlLinks = this._htmlUtils.GetLinksInHtml(html, page.Store.RootUrl);
                this.InsertPageLinks(context, page.StoreId, htmlLinks);

                DatabaseTransactionQueue.Instance.Enqueue(operationKey, transaction);
            }
        }
        private void UpdateFileFromRemote(Guid storeID, String rootUrl, String url, String fullPath)
        {
            String html = this._htmlUtils.GetHtmlContent(url);

            var htmlLinks = this._htmlUtils.GetLinksInHtml(html, rootUrl);

            this.InsertPageLinks(this._context, storeID, htmlLinks);

            FileOperation operation = FileOperation.Create(fullPath, FileOperation.OperationType.CreateOrUpdate, html);
            FileOperationsQueue.Instance.Enqueue(operation);
        }
        private void UpdateReadDate(Page page, DateTime updateDate)
        {
            page.MapStatus = (short)MapStatusType.ContentReady;
            page.LastReadDate = updateDate;
            this._context.Update(page);
        }

    }
}
