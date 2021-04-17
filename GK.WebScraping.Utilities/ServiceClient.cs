using System;
using System.IO;
using System.Net;

namespace GK.WebScraping.Utilities
{
    public class ServiceClient
    {
        private string _uri;

        public ServiceClient(string uri)
        {
            this._uri = uri;
        }

        public String Get()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this._uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public String GetHtml()
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(this._uri);
            }
        }
    }
}
