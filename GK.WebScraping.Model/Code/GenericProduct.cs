using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.Model
{
    public class GenericProduct
    {
        public String Name { get; set; }
        public String ProductID { get; set; }
        public Int32 UnitsInStock { get; set; }
        public Decimal UnitPrice { get; set; }
        public String UnitPriceString { get; set; }
        public String Url { get; set; }
        public Int32? MaxItemCanBePurchased { get; set; }
        public string StoreName { get; set; }
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
