using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GK.WebScraping.Utilities
{
    public class HtmlUtilities
    {

        private static ConnectionClient _client = null;
        private static readonly object _lock = new object();
        private static HashSet<String> _forbiddenExtension = new HashSet<string>(new string[] {
            ".pdf",
            ".tif",
            ".png",
            ".doc",
            ".docx",
            ".jpg",
            ".jpeg",
            ".eps",
            "xls",
            "xlsx"
        });
        private static HashSet<String> _forbiddenPrefixes = new HashSet<string>(new string[] {
            "cookie",
            "?node",
            "ref="
        });
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

        public HashSet<String> GetLinksInHtml(String html, string rootUrl)
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
                if (this.IsValidLink(temp) == false)
                    continue;

                if (String.IsNullOrEmpty(rootUrl) == false &&
                    temp.StartsWith(rootUrl) == false &&
                    temp.StartsWith("/"))
                    temp = rootUrl + temp.Substring(1);


                if (String.IsNullOrEmpty(temp) == false)
                {
                    if (temp.Length > 850)
                        temp = temp.Substring(0, 849);

                    if (temp.StartsWith(rootUrl) &&
                        String.Equals(rootUrl, temp, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        retval.Add(temp);
                    }
                }
            }

            return retval;
        }

        private bool IsValidLink(string temp)
        {
            foreach (String extension in _forbiddenExtension)
            {
                if (temp.EndsWith(extension))
                    return false;
            }
            foreach (String prefix in _forbiddenPrefixes)
            {
                if (temp.StartsWith(prefix))
                    return false;
            }


            return true;
        }

        public String GetHtmlContent(String url)
        {
            //Only use proxy on live...
            return Client.Get(url, Environment.MachineName == "GK-WS1");
        }

        public string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public string EncodeUrlFilename(string url)
        {
            char[] invalids = Path.GetInvalidFileNameChars();

            for (int i = 0; i < invalids.Length; i++)
                url = url.Replace(invalids[i], '_');

            return url;

        }
    }
}
