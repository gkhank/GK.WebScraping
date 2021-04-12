using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.App.Model.Webhallen
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Price
    {
        public string price { get; set; }
        public string currency { get; set; }
        public double vat { get; set; }
        public object type { get; set; }
        public string endAt { get; set; }
        public object maxQtyPerCustomer { get; set; }
    }

    public class Release
    {
        public int timestamp { get; set; }
        public string format { get; set; }
    }

    public class RegularPrice
    {
        public string price { get; set; }
        public string currency { get; set; }
        public double vat { get; set; }
        public object type { get; set; }
        public string endAt { get; set; }
        public object maxQtyPerCustomer { get; set; }
    }

    public class Section
    {
        public int id { get; set; }
        public bool active { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
    }

    public class MainCategoryPath
    {
        public int id { get; set; }
        public string metaTitle { get; set; }
        public object seoName { get; set; }
        public bool active { get; set; }
        public int order { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public bool hasProducts { get; set; }
        public int index { get; set; }
    }

    public class Stock
    {
        public int? web { get; set; }
        public int? supplier { get; set; }
        public int? displayCap { get; set; }

        public int GetTotal()
        {
            return
                    this._1 +
                    this._10 +
                    this._11 +
                    this._14 +
                    this._15 +
                    this._16 +
                    this._17 +
                    this._19 +
                    this._2 +
                    this._20 +
                    this._21 +
                    this._22 +
                    this._23 +
                    this._26 +
                    this._27 +
                    this._28 +
                    this._29 +
                    this._5 +
                    this._8 +
                    this._9;
        }

        [JsonProperty("1")]
        public int _1 { get; set; }
        [JsonProperty("2")]
        public int _2 { get; set; }
        [JsonProperty("15")]
        public int _15 { get; set; }
        [JsonProperty("28")]
        public int _28 { get; set; }
        [JsonProperty("20")]
        public int _20 { get; set; }
        [JsonProperty("21")]
        public int _21 { get; set; }
        [JsonProperty("17")]
        public int _17 { get; set; }
        [JsonProperty("19")]
        public int _19 { get; set; }
        [JsonProperty("16")]
        public int _16 { get; set; }
        [JsonProperty("11")]
        public int _11 { get; set; }
        [JsonProperty("29")]
        public int _29 { get; set; }
        [JsonProperty("9")]
        public int _9 { get; set; }
        [JsonProperty("14")]
        public int _14 { get; set; }
        [JsonProperty("27")]
        public int _27 { get; set; }
        [JsonProperty("23")]
        public int _23 { get; set; }
        [JsonProperty("10")]
        public int _10 { get; set; }
        [JsonProperty("5")]
        public int _5 { get; set; }
        [JsonProperty("22")]
        public int _22 { get; set; }
        [JsonProperty("26")]
        public int _26 { get; set; }
        [JsonProperty("8")]
        public int _8 { get; set; }
    }

    public class AverageRating
    {
        public string rating { get; set; }
        public string ratingType { get; set; }
    }

    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public Price price { get; set; }
        public Release release { get; set; }
        public bool isDigital { get; set; }
        public object energyMarking { get; set; }
        public bool isFyndware { get; set; }
        public object fyndwareClass { get; set; }
        public RegularPrice regularPrice { get; set; }
        public Section section { get; set; }
        public List<MainCategoryPath> mainCategoryPath { get; set; }
        public string categoryTree { get; set; }
        public Stock stock { get; set; }
        public AverageRating averageRating { get; set; }
        public List<int> statusCodes { get; set; }
        public string mainTitle { get; set; }
        public string subTitle { get; set; }
    }

    public class WebhallenResponse
    {
        public List<Product> products { get; set; }
        public int totalProductCount { get; set; }
    }

}
