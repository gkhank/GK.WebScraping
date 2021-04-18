using GK.WebScraping.DB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace GK.WebScraping.Mapper.Service
{
    public class Program
    {


        public static void Main(string[] args)
        {
            //Ensure DB Connection...
            Boolean canConnectDB = TryConnectDB();
            if (canConnectDB)
                CreateHostBuilder(args).Build().Run();
            else
                throw new Exception("Can't connect to database");
        }

        private static bool TryConnectDB()
        {
            using (var context = new WebScrapingContext())
                return context.Database.CanConnect();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddDbContext<WebScrapingContext>();
                     services.AddHostedService<ReaderService>();
                     services.AddMemoryCache();
                     //services.AddHostedService<Mapper>();
                 });
        }
    }
}
