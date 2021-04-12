using GK.WebScraping.Shared.Model;
using System;
using System.Collections.Generic;

namespace GK.WebScraping.App.Utils.Tools
{
    public class HtmlMapper
    {
        public string ProductsNodeSelector { get; }
        private Dictionary<GenericProductColumn, HtmlMap> _map;

        public HtmlMapper(String nodeSelector)
        {
            this.ProductsNodeSelector = nodeSelector;
            this._map = new Dictionary<GenericProductColumn, HtmlMap>();
        }

        internal bool TryGetMap(GenericProductColumn column, out HtmlMap value)
        {
            return this._map.TryGetValue(column, out value);
        }

        internal void Map(GenericProductColumn column, ValueLocation valueLocation, params object[] args)
        {
            this._map.Add(column, new HtmlMap() { ValueLocation = valueLocation, Arguments = Array.ConvertAll<object, String>(args, Convert.ToString) });
        }
    }


    public class HtmlMap
    {
        public String[] Arguments { get; set; }

        public ValueLocation ValueLocation { get; set; }

        public String GetPropertyNodeSelector()
        {
            String retval = String.Join("//", this.Arguments);
            if (retval.StartsWith(".//") == false)
                retval = ".//" + retval;
            return retval;
        }

        public Boolean CanMap
        {
            get
            {
                return this.Arguments.Length > 0 && ValueLocation != ValueLocation.NotAvailable;
            }
        }

    }

    public enum ValueLocation
    {
        InnerHtml,
        ValueAttribute,
        NameAttribute,
        HrefAttribute,
        ClassAttribute,
        Exists,
        NotAvailable,
        DataToItemIdAttribute,
        ContentAttribute
    }

}
