using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.App.Model
{
    public class GenericProduct
    {
        public String Name { get; set; }
        public String ProductID { get; set; }
        public Int32 UnitsInStock { get; set; }
        public Decimal UnitPrice { get; set; }
        public String UnitPriceString { get; set; }
        public String Url { get; internal set; }
        public Int32? MaxItemCanBePurchased { get; internal set; }
        public string StoreName { get; internal set; }
    }

    public enum GenericProductColumn
    {
        Name,
        ProductID,
        UnitsInStock,
        UnitPrice,
        Url,
        MaxItemCanBePurchased,
        StoreName
    }
}
