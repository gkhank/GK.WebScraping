using GK.WebScraping.App.Model.Interface;
using System;

namespace GK.WebScraping.App.Handlers
{
    public class Netonnet : IStore
    {
        public string StoreName => "Netonnet";

        public Netonnet()
        {
        }
        public string UriRoot => "https://www.netonnet.se/search?query={0}&page=1&pageSize=200";

        public string GetProductLink(params object[] args)
        {
            return "https://www.netonnet.se" + args[0];
        }

        public Decimal GetProductPrice(object price)
        {
            if (Decimal.TryParse(price.ToString().Replace(".", ","), out Decimal retval))
                return retval;
            return -1;
        }

        public GenericResponse GetResponse(SearchOptions options)
        {

            ServiceClient client = new ServiceClient(String.Format(this.UriRoot, options.Keyword));

            HtmlMapper mapper = new HtmlMapper("//div[@class='cProductItem col-xs-12 col-sm-4 col-md-6 col-lg-4 product']" );
            mapper.Map(GenericProductColumn.ProductID, ValueLocation.ValueAttribute, "div", "input[@name='ProductId']");
            mapper.Map(GenericProductColumn.Name, ValueLocation.ValueAttribute, "div", "input[@name='ProductName']");
            mapper.Map(GenericProductColumn.UnitPrice, ValueLocation.ValueAttribute, "div", "input[@name='ProductPrice']");
            mapper.Map(GenericProductColumn.UnitsInStock, ValueLocation.Exists, "div", "div", "div[@class='footer  ']", "span[@class='stockStatusInStock']");
            mapper.Map(GenericProductColumn.Url, ValueLocation.HrefAttribute, "div", "div[@class='panel-body leftContent']", "div[@class='smallHeader']", "a");

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
