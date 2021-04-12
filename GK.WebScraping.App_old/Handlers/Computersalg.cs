using GK.WebScraping.App.Model;
using GK.WebScraping.App.Model.Interface;
using GK.WebScraping.App.Utils;
using GK.WebScraping.App.Utils.Tools;
using System;

namespace GK.WebScraping.App.Handlers
{
    public class Computersalg : IStore
    {

        public string UriRoot => "https://www.computersalg.se/l/0/s?sq={0}";

        public string StoreName => "Computersalg";

        public string GetProductLink(params object[] args)
        {
            return "https://www.computersalg.se/" + args[0];
        }

        public Decimal GetProductPrice(object price)
        {
            if (Decimal.TryParse(price.ToString().Replace(".", ","), out Decimal retval))
                return retval;
            return -1;
        }

        public GenericResponse GetResponse(SearchOptions options)
        {
            //String uri = this.UriRoot + options.Keyword;
            //ServiceClient client = new ServiceClient(uri);
            //String raw = client.Get();
            //ComputersalgResponse response = JsonConvert.DeserializeObject<ComputersalgResponse>(raw);
            //return this._adapter.Convert(this, response, options);


            ServiceClient client = new ServiceClient(String.Format(this.UriRoot, options.Keyword));

            HtmlMapper mapper = new HtmlMapper("//div[@class='productContent']");
            mapper.Map(GenericProductColumn.ProductID, ValueLocation.DataToItemIdAttribute, "div[@class='productText']", "a");
            mapper.Map(GenericProductColumn.Name, ValueLocation.InnerHtml, "div[@class='productText']", "a");
            mapper.Map(GenericProductColumn.UnitsInStock, ValueLocation.Exists, "div[@class='productAvailabilityList']", "span[@class='stock green']");
            mapper.Map(GenericProductColumn.Url, ValueLocation.HrefAttribute, "div[@class='productText']", "a");
            mapper.Map(GenericProductColumn.UnitPrice, ValueLocation.ContentAttribute, "div[@class='productAddToCart']", "div[@class='productPrice']", "span[@itemprop='price']");


            HtmlReader reader = new HtmlReader(mapper, client.GetHtml());
            reader.ReadAll();


            GenericResponse retval = new GenericResponse();
            foreach (GenericProduct p in reader.Products)
            {
                p.Url = this.GetProductLink(p.Url);
                p.UnitPrice = this.GetProductPrice(p.UnitPriceString);

                if (((options.OnlyInStock && p.UnitsInStock > 0)
                    || options.OnlyInStock == false) && p.UnitPrice <= options.MaxPrice && p.UnitPrice > options.MinPrice)
                    retval.Products.Add(p);

            }

            return retval;
        }
    }
}
