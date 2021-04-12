using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.App.Model.Computersalg
{
    public class Item
    {
        public object ItemId { get; set; }
        public string ItemHeadline { get; set; }
        public object ItemPictureUrl { get; set; }
        public string ItemLink { get; set; }
        public string OrgQuery { get; set; }
    }

    public class ComputersalgResponse
    {
        public double Exectime { get; set; }
        public string QueryText { get; set; }
        public List<Item> Items { get; set; }
        public List<object> CategorySuggestions { get; set; }
        public string SubmitLink { get; set; }
    }
}
