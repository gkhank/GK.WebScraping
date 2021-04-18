using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GK.WebScraping.DB
{
    public class DatabaseManager
    {
        private static volatile  WebScrapingContext instance = null;
        private static object _lock = new object();

        public static WebScrapingContext WebScraping
        {
            get
            {

                if (instance == null)
                    lock (_lock)
                        if (instance == null)
                            instance = new WebScrapingContext();

                return instance;
            }
        }

    }
}
