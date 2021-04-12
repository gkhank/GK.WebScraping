using GK.WebScraping.App.Model;
using GK.WebScraping.App.Model.Interface;
using GK.WebScraping.App.Model.Komplett;
using GK.WebScraping.App.Utils;
using GK.WebScraping.App.Utils.Adapters;
using Newtonsoft.Json;
using System;

namespace GK.WebScraping.App.Handlers
{
    public class Komplett : IStore
    {
        private readonly IAdapter<KomplettResponse> _adapter;

        public string StoreName => "Komplett";

        public string UriRoot => "https://www.komplett.se/PagesAsync/HeaderSearch/Json?q=";

        public Komplett()
        {
            this._adapter = new KomplettAdapter();
        }

        public string GetProductLink(params object[] args)
        {
            return "https://www.komplett.se/" + args[0];
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
            KomplettResponse response = JsonConvert.DeserializeObject<KomplettResponse>(raw);
            return this._adapter.Convert(this, response, options);
        }
    }
}
