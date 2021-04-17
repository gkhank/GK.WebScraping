using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.DB
{
    public class DatabaseManager
    {
        private static WebScrapingContext instance = null;
        private static readonly object _lock = new object();
        public static WebScrapingContext WebScraping
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new WebScrapingContext();
                    }
                    return instance;
                }
            }
        }

    }
}
