using GK.WebScraping.App.Model.Inet;
using GK.WebScraping.App.Model.Interface;
using GK.WebScraping.App.Utils.Adapters;
using Newtonsoft.Json;
using System;

namespace GK.WebScraping.App.Handlers
{
    class Inet : IStore
    {

        private readonly IAdapter<InetResponse> _adapter;
        public Inet()
        {
            this._adapter = new InetAdapter();
        }

        public string UriRoot => "https://www.inet.se/api/autocomplete?q=";

        public string StoreName => "Inet";

        public string GetProductLink(params object[] args)
        {
            return "https://www.inet.se/produkt/" + args[0];
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
            InetResponse inetResponse = JsonConvert.DeserializeObject<InetResponse>(raw);
            return this._adapter.Convert(this, inetResponse, options);

        }
    }
}
