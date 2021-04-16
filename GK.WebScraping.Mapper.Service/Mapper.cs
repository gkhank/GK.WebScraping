using System;
using System.Threading;
using System.Threading.Tasks;
using GK.WebScraping.DB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using GK.WebScraping.Model;
using System.Collections.Generic;

namespace GK.WebScraping.Mapper.Service
{
    public class Mapper : BackgroundService
    {
        private readonly ILogger<Mapper> _logger;

        public Mapper(ILogger<Mapper> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //List<String> urls =
            //    DatabaseManager.Instance.Pages
            //    .Where(x => x.MapStatus == (Int32)StatusType.None)
            //    .Select(x => x.Url)
            //    .ToList();

            //Start mapping from root...
            List<String> rootUrls =
                DatabaseManager.Instance.Stores
                .Select(x => x.RootUrl).ToList();

            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
