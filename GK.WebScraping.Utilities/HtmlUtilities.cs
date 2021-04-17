using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace GK.WebScraping.Utilities
{
    public class HtmlUtilities
    {
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
            using (Stream stream = this._GetHttpResponse(url).GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private HttpWebResponse _GetHttpResponse(String url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml";
            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            try
            {
                return (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Response == null)
                    throw new Exception("Cannot get response");
                return (HttpWebResponse)e.Response;
            }
        }
    }
}
