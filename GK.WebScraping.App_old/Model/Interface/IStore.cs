using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.App.Model.Interface
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
