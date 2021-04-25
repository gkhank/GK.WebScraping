using GK.WebScraping.Model;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace GK.WebScraping.Utilities
{
    public class ConnectionClient
    {

        private WebProxy _proxy;

        private WebProxy Proxy
        {
            get
            {
                if (this._proxy == null)
                    this._proxy = SelectProxy();
                return this._proxy;
            }
        }


        private WebProxy SelectProxy()
        {
#warning TODO: Implement proxy selection according to this.GetAverage method with uptime response time and location data.


            String[] hosts = File.ReadAllLines(Path.Combine(ApplicationPath.ConfigDirectory, "proxy.txt"));

            //https://openproxy.space/list/ZTABrk0Fn3
            Random rnd = new Random();
            string host = hosts[rnd.Next(0, hosts.Length - 1)];
            String[] pieces = host.Split(':');

            EventLog.WriteEntry(this.ToString(), String.Format("Selected proxy server '{0}'.", host), EventLogEntryType.Information);

            WebProxy retval = new WebProxy(pieces[0], Int32.Parse(pieces[1]));

            if (pieces.Length > 2)
            {
                NetworkCredential credentials = new NetworkCredential(
                    pieces[2],
                    pieces[3]
                );

                retval.Credentials = credentials;
            }


            return retval;
        }

        private double GetAverage(decimal n1, short n2, short n3)
        {
            return Convert.ToDouble((n1 + n2 + n3) / 3);
        }

        public String Get(String url, Boolean useProxy = true)
        {
            try
            {
                if (url.EndsWith(".pdf") ||
                    url.EndsWith(".tif") ||
                    url.EndsWith(".png") ||
                    url.EndsWith(".doc") ||
                    url.EndsWith(".docx") ||
                    url.EndsWith(".jpg") ||
                    url.EndsWith(".jpeg") ||
                    url.EndsWith(".eps"))
                    throw new FormatException(String.Format("File type is not supported for html parsing '{0}'", url));


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = 10000;

                if (useProxy)
                {
                    request.Proxy = this.Proxy;

                    CallResult result = this.DoCall(request);


                    //If call took more than 5 seconds force re-load proxy.
                    if (result.ProcessSpan > new TimeSpan(0, 0, 5))
                    {
                        this._proxy = null;
                    }

                    return result.Content;
                }
                else
                    return this.DoCall(request).Content;

            }
            catch (WebException wex)
            {
                //Force reload proxy if it fails
                //EventLog.WriteEntry(this.ToString(), "Forcing proxy to reload due to WebException.", EventLogEntryType.Information);
                this._proxy = null;
                throw wex;
            }
            catch (FormatException fex)
            {
                throw fex;
            }

        }

        private CallResult DoCall(HttpWebRequest request)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                String content = reader.ReadToEnd();
                watch.Stop();
                return new CallResult(content, watch.Elapsed);
            }
        }

        private Proxy GetProxyFromSource(string url)
        {
            return JsonConvert.DeserializeObject<Proxy>(this.Get(url, false));
        }

        private class CallResult
        {
            public String Content { get; set; }
            public TimeSpan ProcessSpan { get; set; }

            public CallResult(String content, TimeSpan processSpan)
            {
                this.Content = content;
                this.ProcessSpan = processSpan;
            }
        }
    }
}
