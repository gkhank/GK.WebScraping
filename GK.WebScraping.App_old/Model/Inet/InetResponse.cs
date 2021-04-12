using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.App.Model.Inet
{
    public class _00
    {
        public int qty { get; set; }
        public bool blocked { get; set; }
        public DateTime restockDate { get; set; }
        public int restockDays { get; set; }
    }

    public class _01
    {
        public int qty { get; set; }
        public bool blocked { get; set; }
        public int restockDays { get; set; }
    }

    public class _02
    {
        public int qty { get; set; }
        public bool blocked { get; set; }
        public int restockDays { get; set; }
    }

    public class _70
    {
        public int qty { get; set; }
        public bool blocked { get; set; }
        public int restockDays { get; set; }
    }

    public class _71
    {
        public int qty { get; set; }
        public bool blocked { get; set; }
        public int restockDays { get; set; }
    }

    public class InetQuantity
    {
        [JsonProperty("00")]
        public _00 _00 { get; set; }
        [JsonProperty("01")]
        public _01 _01 { get; set; }
        [JsonProperty("02")]
        public _02 _02 { get; set; }
        [JsonProperty("70")]
        public _70 _70 { get; set; }
        [JsonProperty("71")]
        public _71 _71 { get; set; }
    }

    public class InetProduct
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<string> keyText { get; set; }
        public bool active { get; set; }
        public bool hidden { get; set; }
        public DateTime releaseDate { get; set; }
        public double reviewScore { get; set; }
        public int reviewCount { get; set; }
        public double hypeScore { get; set; }
        public int hypeCount { get; set; }
        public string image { get; set; }
        public string urlName { get; set; }
        public InetQuantity qty { get; set; }
        public int qtyLimit { get; set; }
        public string sellingPoint { get; set; }
        public double price { get; set; }
        public double listPrice { get; set; }
        public double priceExVat { get; set; }
        public bool isAssembly { get; set; }
        public bool isBargain { get; set; }
        public bool isPreorder { get; set; }
        public DateTime preorderUntil { get; set; }
        public bool isMemberPrice { get; set; }
        public bool isSwecPrice { get; set; }
    }

    public class InetResponse
    {
        public List<object> categories { get; set; }
        public List<InetProduct> products { get; set; }
        public List<object> campaigns { get; set; }
    }

}
