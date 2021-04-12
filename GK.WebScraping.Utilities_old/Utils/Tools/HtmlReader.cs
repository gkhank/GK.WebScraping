using GK.WebScraping.Shared.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace GK.WebScraping.App.Utils.Tools
{
    public class HtmlReader
    {

        private readonly String _html;

        private List<GenericProduct> _products;

        public IEnumerable<GenericProduct> Products
        {
            get
            {
                return this._products;
            }
        }

        private HtmlAgilityPack.HtmlDocument _doc;

        private HtmlMapper _mapper;

        public Int32 Count { get { return this._products.Count; } }

        public HtmlReader(HtmlMapper mapper, String html)
        {
            this._mapper = mapper;
            this._products = new List<GenericProduct>();
            this._html = html;
        }

        public void ReadAll()
        {
            this._doc = new HtmlAgilityPack.HtmlDocument();
            this._doc.LoadHtml(this._html);

            HtmlNodeCollection productNodes = this._doc.DocumentNode.SelectNodes(this._mapper.ProductsNodeSelector);
            this.InnerReadAll(productNodes);
        }



        private void InnerReadAll(HtmlNodeCollection nodes)
        {
            foreach (HtmlNode n in nodes)
            {
                GenericProduct product = new GenericProduct();


                if (this._mapper.TryGetMap(GenericProductColumn.ProductID, out HtmlMap idMap) == false)
                    throw new Exception("GenericProductColumn.ProductID is not mapped");

                if (this._mapper.TryGetMap(GenericProductColumn.Name, out HtmlMap nameMap) == false)
                    throw new Exception("GenericProductColumn.Name is not mapped");

                if (this._mapper.TryGetMap(GenericProductColumn.UnitPrice, out HtmlMap unitPriceMap) == false)
                    throw new Exception("GenericProductColumn.UnitPrice is not mapped");

                if (this._mapper.TryGetMap(GenericProductColumn.UnitsInStock, out HtmlMap unitsInStockMap) == false)
                    throw new Exception("GenericProductColumn.UnitsInStock is not mapped");

                if (this._mapper.TryGetMap(GenericProductColumn.Url, out HtmlMap urlMap) == false)
                    throw new Exception("GenericProductColumn.Url is not mapped");

                if (idMap.CanMap)
                {
                    HtmlNode idNode = n.SelectSingleNode(idMap.GetPropertyNodeSelector());
                    product.ProductID = this.GetValue(idMap.ValueLocation, idNode);
                }

                if (nameMap.CanMap)
                {
                    HtmlNode nameNode = n.SelectSingleNode(nameMap.GetPropertyNodeSelector());
                    product.Name = this.GetValue(nameMap.ValueLocation, nameNode);
                }

                if (unitPriceMap.CanMap)
                {
                    HtmlNode unitPriceNode = n.SelectSingleNode(unitPriceMap.GetPropertyNodeSelector());
                    product.UnitPriceString = this.GetValue(unitPriceMap.ValueLocation, unitPriceNode);
                }

                if (unitsInStockMap.CanMap)
                {
                    HtmlNode unitsInStockNode = n.SelectSingleNode(unitsInStockMap.GetPropertyNodeSelector());
                    product.UnitsInStock = Convert.ToInt32(this.GetValue(unitsInStockMap.ValueLocation, unitsInStockNode));
                }

                if (urlMap.CanMap)
                {
                    HtmlNode urlNode = n.SelectSingleNode(urlMap.GetPropertyNodeSelector());
                    product.Url = this.GetValue(urlMap.ValueLocation, urlNode);
                }

                this._products.Add(product);

            }

        }

        private String GetValue(ValueLocation valueLocation, HtmlNode node)
        {
            switch (valueLocation)
            {
                case ValueLocation.InnerHtml:
                    return node.InnerHtml;
                case ValueLocation.ValueAttribute:
                    return node.Attributes["value"].Value;
                case ValueLocation.NameAttribute:
                    return node.Attributes["name"].Value;
                case ValueLocation.HrefAttribute:
                    return node.Attributes["href"].Value;
                case ValueLocation.ClassAttribute:
                    return node.Attributes["class"].Value;
                case ValueLocation.Exists:
                    return node != null ? "1" : "0";
                case ValueLocation.DataToItemIdAttribute:
                    return node.Attributes["data-toitemid"].Value;
                case ValueLocation.NotAvailable:
                    return String.Empty;
                case ValueLocation.ContentAttribute:
                    return node.Attributes["content"].Value;
                default:
                    throw new NotImplementedException();
            }
        }


        private bool TryGetNodeID(HtmlNode n, out string nodeKey)
        {
            nodeKey = null;
            if (n.Attributes["key"] != null)
                nodeKey = n.Attributes["key"].Value;
            if (String.IsNullOrEmpty(nodeKey) && n.Attributes["id"] != null)
                nodeKey = n.Attributes["id"].Value;
            if (String.IsNullOrEmpty(nodeKey) && n.Attributes["class"] != null)
                nodeKey = n.Attributes["class"].Value;
            return !String.IsNullOrEmpty(nodeKey);

        }

    }
}
