using System;

namespace DNIC.Models
{
    public class PcBuildProduct
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid PcBuildId { get; set; }
        public PcBuild PcBuild { get; set; }

        public PcBuildProduct(Guid productId, Guid pcBuildId)
        {
            ProductId = productId;
            PcBuildId = pcBuildId;
        }
    }
}
