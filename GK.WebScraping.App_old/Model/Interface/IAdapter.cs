using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.App.Model.Interface
{
    public interface IAdapter<ResponseType>
    {
        public GenericResponse Convert(IStore store, ResponseType response, SearchOptions options);
    }
}
