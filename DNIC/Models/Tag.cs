using System;

namespace DNIC.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Tag(string name, Guid productId)
        {
            Name = name;
            ProductId = productId;
        }
    }
}
