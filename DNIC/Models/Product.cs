using System;
using System.Collections.Generic;

namespace DNIC.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Price { get; set; }

        public string Url { get; set; }

        public int RealId { get; set; }

        public string StoreName { get; set; }

        public string StoreImageUrl { get; set; }

        public ICollection<PcBuildProduct> PcBuildProducts { get; set; }

        public virtual List<Tag> Tags { get; set; }
    }
}
