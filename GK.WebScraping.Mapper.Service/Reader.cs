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

namespace GK.WebScraping.Mapper.Service
{
    public class Reader : BackgroundService
    {
        private readonly ILogger<Reader> _logger;
        private readonly HtmlUtilities _htmlUtils;
        public Reader(ILogger<Reader> logger)
        {
            this._logger = logger;
            this._htmlUtils = new HtmlUtilities();
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);


            Dictionary<Guid, Store> storesById = DatabaseManager.WebScraping.Stores.ToDictionary(x => x.StoreId);

            while (!stoppingToken.IsCancellationRequested)
            {
                //Prepare links for read operation. Exclude pages that we previously read.
                DateTime lastAcceptableReadDate = DateTime.Now.AddDays(-1);

                var pages = DatabaseManager.WebScraping.Pages
                    .Where(x => x.LastReadDate.HasValue == false || x.LastReadDate <= lastAcceptableReadDate || String.IsNullOrEmpty(x.Content)).ToDictionary(x => x.Url);

                Boolean requireSave = false;

                //Read contents for given url set.
                foreach (var page in pages.Values)
                {
                    this.ForceUpdateExistingPage(page);

                    requireSave |= true;

                    //Read all links for every content and save them to database to be read on next execution.

                    if (storesById.TryGetValue(page.StoreId, out Store store) == false)
                    {
                        this._logger.LogWarning("Could not relate any store with storeId '{0}'", page.StoreId);
                        continue;
                    }


                    HashSet<String> pageUrls = this._htmlUtils.GetLinksInHtml(page.Content, store.RootUrl);


                    //Exclude pages that read before and still valid. Check "lastAcceptableReadDate".
                    Dictionary<String, Page> existingUrls = DatabaseManager.WebScraping.Pages.Where(x => pageUrls.Contains(x.Url)).ToDictionary(x => x.Url);


                    foreach (String pageUrl in pageUrls)
                    {

                        //Exists in database
                        if (existingUrls.TryGetValue(pageUrl, out Page existingPage))
                        {

                            //Check if we need to read it again...
                            if ((page.LastReadDate.HasValue && page.LastReadDate <= lastAcceptableReadDate) ||
                                page.LastReadDate.HasValue == false ||
                                String.IsNullOrEmpty(page.Content))
                            {
                                this.ForceUpdateExistingPage(existingPage);
                            }
                        }
                        //New page.
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

                            requireSave |= true;
                            DatabaseManager.WebScraping.Pages.Add(newPage);
                        }



                    }

                }

                if (requireSave)
                    await DatabaseManager.WebScraping.SaveChangesAsync();

            }
        }

        private void ForceUpdateExistingPage(Page page)
        {
            //Update content
            page.Content = this._htmlUtils.GetHtmlContent(page.Url);
            page.MapStatus = (short)MapStatusType.ContentReady;
            page.LastReadDate = DateTime.Now;
            DatabaseManager.WebScraping.Entry(page).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
