using System;
using System.Collections.Generic;

#nullable disable

namespace GK.WebScraping.Model
{
    public partial class Page
    {
        public Page()
        {
            Maps = new HashSet<Map>();
        }

        public Guid PageId { get; set; }
        public Guid StoreId { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public DateTime? LastReadDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public short Status { get; set; }
        public short MapStatus { get; set; }

        public virtual Store Store { get; set; }
        public virtual ICollection<Map> Maps { get; set; }
    }
}
