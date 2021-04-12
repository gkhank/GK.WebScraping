using System;
using System.Collections.Generic;

#nullable disable

namespace GK.WebScraping.Model
{
    public partial class Field
    {
        public Field()
        {
            Maps = new HashSet<Map>();
            ProductData = new HashSet<ProductDatum>();
        }

        public int FieldId { get; set; }
        public string Name { get; set; }
        public short? Status { get; set; }
        public DateTime CreateDate { get; set; }
        public short FieldType { get; set; }
        public short DataType { get; set; }

        public virtual ICollection<Map> Maps { get; set; }
        public virtual ICollection<ProductDatum> ProductData { get; set; }
    }
}
