using GK.WebScraping.DB;
using GK.WebScraping.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GK.WebScraping.Test
{
    [TestClass]
    public class DatabaseConnectionTest
    {
        [TestMethod]
        public void TestConnection()
        {
            Store[] stores = DatabaseManager.WebScraping.Stores.ToArray();
            Assert.IsNotNull(stores);
        }
    }
}
