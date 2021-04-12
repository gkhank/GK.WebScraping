using System;
using System.Collections.Generic;

namespace GK.WebScraping.Model
{
    public class SearchOptions
    {
        public String Keyword { get; set; }
        public Boolean OnlyInStock { get; set; }
        public Int32 WaitSeconds { get; set; }
        public int RunHours { get; set; }
        public HashSet<string> SelectedStores { get; set; }
        public Int32? RunCount { get; set; }
        public Decimal MaxPrice { get; set; }
        public Decimal MinPrice { get; set; }
    }
}
