using System;
using System.Collections.Generic;

#nullable disable

namespace GK.WebScraping.Model
{
    public partial class Store
    {
        public Store()
        {
            Pages = new HashSet<Page>();
        }

        public Guid StoreId { get; set; }
        public string Name { get; set; }
        public string RootUrl { get; set; }
        public short Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        public virtual ICollection<Page> Pages { get; set; }
    }
}
