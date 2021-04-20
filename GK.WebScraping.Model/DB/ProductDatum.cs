using System;
using System.Collections.Generic;

#nullable disable

namespace GK.WebScraping.Model
{
    public partial class ProductDatum
    {
        public int ProductId { get; set; }
        public int FieldId { get; set; }
        public int? IntData { get; set; }
        public string StringData { get; set; }
        public DateTime DateTimeData { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Field Field { get; set; }
        public virtual Product Product { get; set; }
    }
}
