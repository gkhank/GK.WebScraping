using System;
using System.Collections.Generic;

#nullable disable

namespace GK.WebScraping.Model
{
    public partial class Map
    {
        public int FieldId { get; set; }
        public Guid PageId { get; set; }
        public DateTime CreateDate { get; set; }
        public short Status { get; set; }
        public string Map1 { get; set; }

        public virtual Field Field { get; set; }
        public virtual Page Page { get; set; }
    }
}
