using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.Model

{
    public class GenericResponse
    {
        public List<GenericProduct> Products { get; set; }
        public GenericResponse()
        {
            this.Products = new List<GenericProduct>();

        }
        public void Add(GenericProduct p)
        {
            this.Products.Add(p);
        }
    }
}
