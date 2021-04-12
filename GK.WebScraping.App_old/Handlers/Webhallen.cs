using GK.WebScraping.App.Model;
using GK.WebScraping.App.Model.Interface;
using GK.WebScraping.App.Model.Webhallen;
using GK.WebScraping.App.Utils;
using GK.WebScraping.App.Utils.Adapters;
using Newtonsoft.Json;
using System;

namespace GK.WebScraping.App.Handlers
{
    public class Webhallen : IStore
    {
        private readonly IAdapter<WebhallenResponse> _adapter;
        public Webhallen()
        {
            this._adapter = new WebhallenAdapter();
        }


        public string UriRoot => "https://www.webhallen.com/api/search/live-results/";

        public string StoreName => "Webhallen";

        public string GetProductLink(params object[] args)
        {
            return "https://www.webhallen.com/se/product/" + args[0];
        }

        public Decimal GetProductPrice(object price)
        {
            if (Decimal.TryParse(price.ToString().Replace(".", ","), out Decimal retval))
                return retval;
            return -1;
        }

        public GenericResponse GetResponse(SearchOptions options)
        {
            String uri = this.UriRoot + options.Keyword;
            ServiceClient client = new ServiceClient(uri);
            String raw = client.Get();
            WebhallenResponse webhallenResponse = JsonConvert.DeserializeObject<WebhallenResponse>(raw);
            return this._adapter.Convert(this, webhallenResponse, options);
        }
    }
}
