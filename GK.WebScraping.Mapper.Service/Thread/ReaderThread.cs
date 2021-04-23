using GK.WebScraping.DB;
using GK.WebScraping.Mapper.Service.Queues;
using GK.WebScraping.Model;
using GK.WebScraping.Model.Code.Operations;
using GK.WebScraping.Utilities;
using Microsoft.Data.SqlClient;
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
        private static readonly object _webClientLock = new object();
        private DateTime _lastAcceptableReadDate;

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

                Dictionary<Int32, Page> pages = this.GetPagesToProcess();

                //If there is no pages to read. Sleep thread until next time...
                if (pages == null ||
                    pages.Count == 0)
                {
                    DateTime sleepUntil = this.NextRunTime();
                    this.SleepThread(sleepUntil);
                }

                try
                {
                    foreach (var page in pages.Values)
                    {
                        this.Update(page);
                    }

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
            DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.Low);
            using (WebScrapingContext context = DatabaseTransactionQueue.Instance.GetContext(operationKey))
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                return context.Pages.Min(x => x.LastReadDate).Value.AddDays(1);
            }

        }
        private Dictionary<Int32, Page> GetPagesToProcess()
        {

            Boolean lockTaken = false;
            try
            {
                Monitor.Enter(_dbLock, ref lockTaken);

                if (lockTaken)
                {

                    DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.Low);
                    using (WebScrapingContext context = DatabaseTransactionQueue.Instance.GetContext(operationKey))
                    using (IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        context.Database.OpenConnection();

                        String sql = $@"EXEC sp_GetPagesToProcess 
                                                        {this._bulkSize}, 
                                                        '{this._lastAcceptableReadDate:yyyy-MM-dd HH:mm:ss}', 
                                                        {(Int32)MapStatusType.ContentInProgress}, 
                                                        @excludingMap, 
                                                        {0}";
                        //Debug.Write(sql);
                        var result = context.Pages.FromSqlRaw(sql, tvp_IntTable.GenerateSqlParameter("@excludingMap", new object[] { MapStatusType.ContentInProgress, MapStatusType.MappingInProgress }));
                        #region Raw sql solution (DO NOT USE)
                        //        context.Pages.FromSqlRaw($@"
                        //SELECT TOP {this._bulkSize}
                        // * 
                        //FROM 
                        // [Page]
                        // WITH (XLOCK)
                        //WHERE
                        // [Page].status = 1
                        // AND
                        // [Page].deleteDate IS NULL
                        // AND
                        // (
                        //  [Page].lastReadDate IS NULL
                        //  OR
                        //  [Page].lastReadDate < '{dateString}'
                        // )
                        //")
                        #endregion
                        #region Linq solution (DO NOT USE)
                        //context.Pages
                        //.Where(
                        //x => x.DeleteDate.HasValue == false &&
                        //x.Status == (short)StatusType.Active &&
                        //(x.LastReadDate.HasValue == false || x.LastReadDate.Value < this._lastAcceptableReadDate))
                        #endregion


                        //Try to replace it with .Include if they add support to FromSqlRaw().Include in the future.
                        #region Tidious store inclusion
                        HashSet<Guid> storeIDs = new HashSet<Guid>();
                        Dictionary<Int32, Page> retval = new Dictionary<int, Page>();
                        foreach (var item in result)
                        {
                            storeIDs.Add(item.StoreId);
                            retval.Add(item.PageId, item);
                        }

                        Dictionary<Guid, Store> relatedStoresById = context.Stores.Where(x => storeIDs.Contains(x.StoreId)).ToDictionary(x => x.StoreId);

                        foreach (Page item in retval.Values)
                            if (relatedStoresById.TryGetValue(item.StoreId, out Store store))
                                item.Store = store;
                        #endregion
                        return retval;
                    }
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
                    Monitor.Exit(_dbLock);
                }
            }

        }
        private Dictionary<String, Page> GetDuplicatePages(WebScrapingContext context, HashSet<String> pageUrls)
        {

            Boolean lockTaken = false;
            try
            {
                Monitor.Enter(_dbLock, ref lockTaken);

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
                    Monitor.Exit(_dbLock);
                }
            }

        }
        private void CommitPageLinks(Guid storeId, HashSet<string> pageUrls)
        {
            if (pageUrls.Count <= 0)
                return;

            Boolean lockTaken = false;

            try
            {
                Monitor.Enter(_dbLock, ref lockTaken);

                if (lockTaken)
                {
                    DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.Low);

                    using (WebScrapingContext context = DatabaseTransactionQueue.Instance.GetContext(operationKey))
                    using (IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                    {
                        context.Database.ExecuteSqlRaw("EXEC sp_CommitPageLinks @links, @storeID",
                                tvp_StringTable.GenerateSqlParameter("@links", pageUrls.ToArray()),
                                new SqlParameter("@storeID", storeId.ToString()));
                        transaction.Commit();
                        context.SaveChanges();
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_dbLock);
                }
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
                this.UpdateReadDate(page, DateTime.Now);

                HashSet<String> htmlLinks = this._htmlUtils.GetLinksInHtml(html, page.Store.RootUrl);
                this.CommitPageLinks(page.StoreId, htmlLinks);
            }
        }

        private void UpdateFileFromRemote(Guid storeID, String rootUrl, String url, String fullPath)
        {

            Boolean lockTaken = false;

            try
            {
                Monitor.Enter(_webClientLock, ref lockTaken);

                if (lockTaken)
                {
                    String html = this._htmlUtils.GetHtmlContent(url);

                    var htmlLinks = this._htmlUtils.GetLinksInHtml(html, rootUrl);

                    this.CommitPageLinks(storeID, htmlLinks);

                    FileOperation operation = FileOperation.Create(fullPath, FileOperation.OperationType.CreateOrUpdate, html);
                    FileOperationsQueue.Instance.Enqueue(operation);
                }
            }
            finally
            {
                Monitor.Exit(_webClientLock);
            }
        }
        private void UpdateReadDate(Page page, DateTime updateDate)
        {

            Boolean lockTaken = false;

            try
            {
                Monitor.Enter(_dbLock, ref lockTaken);

                if (lockTaken)
                {
                    DatabaseProcessKey operationKey = DatabaseProcessKey.GenerateKey(PriorityType.Low);

                    using (WebScrapingContext context = DatabaseTransactionQueue.Instance.GetContext(operationKey))
                    using (IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                    {

                        page.MapStatus = (short)MapStatusType.ContentReady;
                        page.LastReadDate = updateDate;
                        context.Update(page);
                        transaction.Commit();
                        context.SaveChanges();
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_dbLock);
                }
            }

        }

    }
}
