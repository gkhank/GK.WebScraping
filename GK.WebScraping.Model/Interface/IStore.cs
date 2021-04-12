using System;

namespace GK.WebScraping.Model.Interface
{
    public interface IStore
    {
        public String StoreName { get; }
        public String UriRoot { get; }
        public GenericResponse GetResponse(SearchOptions options);
        public Decimal GetProductPrice(object price);
        public String GetProductLink(params object[] args);
    }
}
