using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.App.Model.Komplett
{
    public class Product
    {
        public string id { get; set; }
        public string imageSrc { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public string url { get; set; }
        public bool priceOnRequest { get; set; }
        public bool isTelia { get; set; }
    }

    public class ProductGroup
    {
        public string category { get; set; }
        public string url { get; set; }
        public List<Product> products { get; set; }
    }


    public class KomplettResponse
    {
        public int hits { get; set; }
        public bool showAsNetPrices { get; set; }
        public List<ProductGroup> productGroups { get; set; }
        public List<object> sellers { get; set; }
    }


}
