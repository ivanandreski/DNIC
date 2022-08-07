using System;
using System.Collections.Generic;

namespace DNIC.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Price { get; set; }

        public bool IsAvailable { get; set; }

        public string StoreName { get; set; }

        public byte[] StoreImage { get; set; }

        public ICollection<PcBuildProduct> PcBuildProducts { get; set; }

        public virtual List<Tag> Tags { get; set; }
    }
}
