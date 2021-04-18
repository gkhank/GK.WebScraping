using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GK.WebScraping.Utilities
{
    public class HtmlUtilities
    {

        private static ConnectionClient _client = null;
        private static readonly object _lock = new object();
        public static ConnectionClient Client
        {
            get
            {
                lock (_lock)
                {
                    if (_client == null)
                    {
                        _client = new ConnectionClient();
                    }
                    return _client;
                }
            }
        }

        public HashSet<String> GetLinksInHtml(String html, string rootUrl = null)
        {
            if (String.IsNullOrEmpty(html))
                return new HashSet<string>();


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HashSet<String> retval = new HashSet<string>();

            var links = doc.DocumentNode.Descendants("a")
                .Select(x => x.GetAttributeValue<String>("href", null))
                .Where(x => String.IsNullOrEmpty(x) == false && x.StartsWith("#") == false && x != rootUrl);

            foreach (String l in links)
            {
                String temp = l;
                if (String.IsNullOrEmpty(rootUrl) == false &&
                    temp.StartsWith(rootUrl) == false &&
                    temp.StartsWith("/"))
                    temp = rootUrl + temp.Substring(1);


                if (String.IsNullOrEmpty(temp) == false)
                {
                    if (temp.Length > 850)
                        temp = temp.Substring(0, 849);

                    if (temp.StartsWith(rootUrl) &&
                        retval.Contains(temp) == false)
                    {
                        retval.Add(temp);
                    }
                }
            }

            return retval;
        }

        public String GetHtmlContent(String url)
        {
            //Only use proxy on live...
            return Client.Get(url, Environment.MachineName == "GK-WS1");
        }
    }
}
