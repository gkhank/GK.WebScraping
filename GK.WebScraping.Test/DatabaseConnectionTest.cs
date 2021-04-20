using GK.WebScraping.DB;
using GK.WebScraping.Model;
using GK.WebScraping.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace GK.WebScraping.Test
{
    [TestClass]
    public class DatabaseConnectionTest
    {
        [TestMethod]
        public void WebScrapingConnectionTest()
        {

            using (WebScrapingContext context = new WebScrapingContext())
            {
                Assert.IsTrue(context.Database.CanConnect());
            }
        }



        [TestMethod]
        public void DatabaseQueueManagerSingleThreadWriteTest()
        {

            //Init database

            var provider = new ServiceCollection()
                       .AddMemoryCache()
                       .BuildServiceProvider();


            var options = new DbContextOptionsBuilder<WebScrapingContext>()
                    //.UseMemoryCache(provider.GetService<IMemoryCache>())
                    .UseSqlServer("Server=localhost;Database=WebScraping;Trusted_Connection=True")
                    .Options;
            DatabaseProcessKey key = DatabaseProcessKey.GenerateKey(PriorityType.Normal);


            WebScrapingContext context = DatabaseTransactionQueue.Instance.GetContext(key, options);
            context.Database.AutoTransactionsEnabled = false;

            Assert.IsTrue(context.Database.CanConnect());

            IDbContextTransaction writeTransaction = context.Database.BeginTransaction();
            Random rnd = new Random();

            Int32 lenght = rnd.Next(3, 10);

            for (int i = 0; i < lenght; i++)
            {

                Page newPage = new Page()
                {
                    LastReadDate = null,
                    CreateDate = DateTime.Now,
                    DeleteDate = DateTime.Now,
                    MapStatus = (short)MapStatusType.None,
                    Url = "testpage_" + i,
                    Status = (short)StatusType.Active,
                    StoreId = Guid.Parse("F7BF1777-8275-4879-BC6B-B2B8A8387489")
                };

                context.Pages.Add(newPage);
            }


            DatabaseTransactionQueue.Instance.Enqueue(
                DatabaseProcessKey.GenerateKey(PriorityType.High),
                writeTransaction);


        }

        [TestMethod]
        public void DatabaseQueueManagerMultiThreadWriteTest()
        {
            Guid thread1Id = Guid.NewGuid();
            Guid thread2Id = Guid.NewGuid();



            Thread thread1 = new Thread(new ParameterizedThreadStart(this.WriteOperation));
            Thread thread2 = new Thread(new ParameterizedThreadStart(this.WriteOperation));

            Timer timer = new Timer(x =>
            {
                thread1.Abort();
                thread2.Abort();

            }, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));


            thread1.Start(thread1Id);
            thread1.Start(thread2Id);

        }


        private void ReadOperation(object obj)
        {
            DatabaseProcessKey key = DatabaseProcessKey.GenerateKey(PriorityType.Normal);


            WebScrapingContext context = DatabaseTransactionQueue.Instance.GetContext(key);
            IDbContextTransaction transaction = context.Database.BeginTransaction();

            Random rnd = new Random();
            Int32 lenght = rnd.Next(20, 50);

            for (int i = 0; i < lenght; i++)
            {

                var result = context.Pages.Where(x => x.Url.StartsWith("testpage_") && x.Url.EndsWith("_" + i) && x.DeleteDate.HasValue);
            }

            DatabaseTransactionQueue.Instance.Enqueue(
                DatabaseProcessKey.GenerateKey(PriorityType.Normal),
                transaction);
        }

        private void WriteOperation(object obj)
        {
            DatabaseProcessKey key = DatabaseProcessKey.GenerateKey(PriorityType.Normal);

            //Init database

            var provider = new ServiceCollection()
                       .AddMemoryCache()
                       .BuildServiceProvider();


            var options = new DbContextOptionsBuilder<WebScrapingContext>()
                    //.UseMemoryCache(provider.GetService<IMemoryCache>())
                    .UseSqlServer("Server=localhost;Database=WebScraping;Trusted_Connection=True")
                    .Options;


            WebScrapingContext context = DatabaseTransactionQueue.Instance.GetContext(key, options);
            IDbContextTransaction transaction = context.Database.BeginTransaction();

            Random rnd = new Random();

            Int32 lenght = rnd.Next(1, 5);

            for (int i = 0; i < lenght; i++)
            {

                Page newPage = new Page()
                {
                    LastReadDate = null,
                    CreateDate = DateTime.Now,
                    DeleteDate = DateTime.Now,
                    MapStatus = (short)MapStatusType.None,
                    Url = "testpage_" + i,
                    Status = (short)StatusType.Active,
                    StoreId = Guid.Parse("F7BF1777-8275-4879-BC6B-B2B8A8387489")
                };

                context.Pages.Add(newPage);
            }

            DatabaseTransactionQueue.Instance.Enqueue(key, transaction);


        }
    }
}
