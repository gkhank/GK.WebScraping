using GK.WebScraping.DB;
using GK.WebScraping.Model;
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
            Guid thread1Id = Guid.NewGuid();

            //Init database

            var provider = new ServiceCollection()
                       .AddMemoryCache()
                       .BuildServiceProvider();


            var options = new DbContextOptionsBuilder<WebScrapingContext>()
                    //.UseMemoryCache(provider.GetService<IMemoryCache>())
                    .UseSqlServer("Server=localhost;Database=WebScraping;Trusted_Connection=True")
                    .Options;


            WebScrapingContext context = DatabaseTransactionQueueManager.Instance.GetContext(thread1Id, options);
            context.Database.AutoTransactionsEnabled = false;

            Assert.IsTrue(context.Database.CanConnect());

            IDbContextTransaction writeTransaction = context.Database.BeginTransaction();
            Random rnd = new Random();

            Int32 lenght = rnd.Next(3, 10);

            for (int i = 0; i < lenght; i++)
            {

                Page newPage = new Page()
                {
                    PageId = Guid.NewGuid(),
                    LastReadDate = null,
                    CreateDate = DateTime.Now,
                    DeleteDate = DateTime.Now,
                    MapStatus = (short)MapStatusType.None,
                    Url = "testpage_" + thread1Id + "_" + i,
                    Status = (short)StatusType.Active,
                    StoreId = Guid.Parse("F7BF1777-8275-4879-BC6B-B2B8A8387489")
                };

                context.Pages.Add(newPage);
            }


            DatabaseTransactionQueueManager.Instance.QueueTransaction(
                DatabaseProcessKey.GenerateKey(DatabaseQueuePriorityType.High),
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
            Guid threadID = Guid.Parse(obj.ToString());

            WebScrapingContext context = DatabaseTransactionQueueManager.Instance.GetContext(threadID);
            IDbContextTransaction transaction = context.Database.BeginTransaction();

            Random rnd = new Random();
            Int32 lenght = rnd.Next(20, 50);

            for (int i = 0; i < lenght; i++)
            {

                var result = context.Pages.Where(x => x.Url.StartsWith("testpage_") && x.Url.EndsWith("_" + i) && x.DeleteDate.HasValue);
            }

            DatabaseTransactionQueueManager.Instance.QueueTransaction(
                DatabaseProcessKey.GenerateKey(DatabaseQueuePriorityType.Normal),
                transaction);
        }

        private void WriteOperation(object obj)
        {
            Guid threadID = Guid.Parse(obj.ToString());

            //Init database

            var provider = new ServiceCollection()
                       .AddMemoryCache()
                       .BuildServiceProvider();


            var options = new DbContextOptionsBuilder<WebScrapingContext>()
                    //.UseMemoryCache(provider.GetService<IMemoryCache>())
                    .UseSqlServer("Server=localhost;Database=WebScraping;Trusted_Connection=True")
                    .Options;


            WebScrapingContext context = DatabaseTransactionQueueManager.Instance.GetContext(threadID, options);
            IDbContextTransaction transaction = context.Database.BeginTransaction();

            Random rnd = new Random();

            Int32 lenght = rnd.Next(1, 5);

            for (int i = 0; i < lenght; i++)
            {

                Page newPage = new Page()
                {
                    PageId = Guid.NewGuid(),
                    LastReadDate = null,
                    CreateDate = DateTime.Now,
                    DeleteDate = DateTime.Now,
                    MapStatus = (short)MapStatusType.None,
                    Url = "testpage_" + threadID + "_" + i,
                    Status = (short)StatusType.Active,
                    StoreId = Guid.Parse("F7BF1777-8275-4879-BC6B-B2B8A8387489")
                };

                context.Pages.Add(newPage);
            }

            DatabaseTransactionQueueManager.Instance.QueueTransaction(
                new DatabaseProcessKey(threadID, DatabaseQueuePriorityType.High),
                transaction);


        }
    }
}
