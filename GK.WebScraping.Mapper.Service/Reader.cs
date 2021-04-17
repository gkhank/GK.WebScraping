using System;
using System.Threading;
using System.Threading.Tasks;
using GK.WebScraping.DB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using GK.WebScraping.Utilities;
using GK.WebScraping.Model;
using System.IO;
using System.Diagnostics;

namespace GK.WebScraping.Mapper.Service
{
    public class Reader : BackgroundService
    {
        private readonly ILogger<Reader> _logger;
        private readonly HtmlUtilities _htmlUtils;
        private static readonly object _lock = new object();
        private DateTime _lastAcceptableReadDate;

        public Reader(ILogger<Reader> logger)
        {
            this._logger = logger;
            this._htmlUtils = new HtmlUtilities();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
            this.Execute(stoppingToken);
        }

        private void Execute(CancellationToken stoppingToken)
        {
            Dictionary<Guid, Store> storesById = DatabaseManager.WebScraping.Stores.ToDictionary(x => x.StoreId);

            while (!stoppingToken.IsCancellationRequested)
            {
                lock (_lock)
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    //Prepare links for read operation. Exclude pages that we previously read.
                    this._lastAcceptableReadDate = DateTime.Now.AddDays(-1);

                    var pages = DatabaseManager.WebScraping.Pages
                        .Where(x => x.DeleteDate.HasValue == false && x.Status == (Int32)StatusType.Active &&
                        (x.LastReadDate.HasValue == false || x.LastReadDate <= this._lastAcceptableReadDate || String.IsNullOrEmpty(x.Content)))
                        .ToDictionary(x => x.Url);

                    Int32 updateCount = 0, insertCount = 0;
                    //Read contents for given url set.


                    foreach (var page in pages.Values)
                    {
                        String html = null;

                        this.UpdateDatabaseAndFile(page, ref html);
                        updateCount++;
                        //Read all links for every content and save them to database to be read on next execution.

                        if (storesById.TryGetValue(page.StoreId, out Store store) == false)
                        {
                            this._logger.LogWarning("Could not relate any store with storeId '{0}'", page.StoreId);
                            continue;
                        }

                        HashSet<String> pageUrls = this._htmlUtils.GetLinksInHtml(html, store.RootUrl);

                        //Exclude pages that read before and still valid. Check "lastAcceptableReadDate".
                        Dictionary<String, Page> databasePages = DatabaseManager.WebScraping.Pages.Where(x => pageUrls.Contains(x.Url)).ToDictionary(x => x.Url);
                        Dictionary<String, Page> processedPages = new Dictionary<string, Page>();

                        foreach (String pageUrl in pageUrls)
                        {

                            //Get existing urls after handling each pages.

                            //Is already processed in this thread
                            if (processedPages.ContainsKey(pageUrl))
                                continue;

                            //Exists in database
                            else if (databasePages.TryGetValue(pageUrl, out Page existingPage))
                            {
                                //Check if we need to read it again...
                                if ((page.LastReadDate.HasValue && page.LastReadDate <= this._lastAcceptableReadDate) ||
                                    page.LastReadDate.HasValue == false)
                                {
                                    String tempHtml = null;
                                    this.UpdateDatabaseAndFile(existingPage, ref tempHtml);
                                    updateCount++;
                                }
                            }
                            //Not in database and not processed eforeNew page.
                            else
                            {
                                Page newPage = new Page()
                                {
                                    PageId = Guid.NewGuid(),
                                    Content = null, //Will be read on next execution.
                                    LastReadDate = null,
                                    CreateDate = DateTime.Now,
                                    DeleteDate = null,
                                    MapStatus = (short)MapStatusType.None,
                                    Url = pageUrl,
                                    Status = (short)StatusType.Active,
                                    StoreId = page.StoreId //Assign same storeId from parent page.
                                };

                                processedPages.Add(pageUrl, newPage);
                            }

                        }

                        //Add and save processed after every page
                        if (processedPages.Count > 0)
                            DatabaseManager.WebScraping.Pages.AddRange(processedPages.Values);

                        if (DatabaseManager.WebScraping.ChangeTracker.HasChanges())
                            DatabaseManager.WebScraping.SaveChanges();
                    }


                    watch.Stop();

                    this._logger.LogInformation("Processed {0} update and {1} insert operations in {2}ms", updateCount, insertCount, watch.ElapsedMilliseconds);

                    if (DatabaseManager.WebScraping.ChangeTracker.HasChanges())
                        DatabaseManager.WebScraping.SaveChanges();
                }

            }

        }

        private void UpdateDatabaseAndFile(Page page, ref string html)
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
                        this.UpdateFile(page, fullPath, ref html);
                    else
                    {
                        this._logger.LogWarning("Attempted to read HTML unneccsarily for '{0}'. Using existing content.", page.Url);
                        html = this.ReadFile(fullPath);
                    }
                }
                else
                {
                    this.UpdateFile(page, fullPath, ref html);
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

        private void UpdateFile(Page page, String fullPath, ref String html)
        {

            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    html = this._htmlUtils.GetHtmlContent(page.Url);
                    writer.Write(html);

                    writer.Dispose();
                }
                fs.Dispose();
            }

            page.MapStatus = (short)MapStatusType.ContentReady;
            page.LastReadDate = DateTime.Now;
            DatabaseManager.WebScraping.Entry(page).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
