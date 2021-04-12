using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GK.WebScraping.App.Model
{
    public class SearchOptions
    {
        public String Keyword { get; set; }
        public Boolean OnlyInStock { get; set; }
        public Int32 WaitSeconds { get; internal set; }
        public int RunHours { get; internal set; }
        public HashSet<string> SelectedStores { get; internal set; }
        public Int32? RunCount { get; internal set; }
        public Decimal MaxPrice { get; internal set; }
        public Decimal MinPrice { get; internal set; }
    }
}
