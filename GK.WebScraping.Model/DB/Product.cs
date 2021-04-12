using System;
using System.Collections.Generic;

#nullable disable

namespace GK.WebScraping.Model
{
    public partial class Product
    {
        public Product()
        {
            ProductData = new HashSet<ProductDatum>();
        }

        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual ICollection<ProductDatum> ProductData { get; set; }
    }
}
