using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DNIC.Models
{
    public class PcBuild : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<PcBuildProduct> PcBuildProducts { get; set; }

        public int MotherboardProccesorCompatibility()
        {
            var motherboard = PcBuildProducts.Select(x => x.Product).FirstOrDefault(x => x.Type == ProductTypes.Motherboard);
            if (motherboard == null) return 0;

            var proccesor = PcBuildProducts.Select(x => x.Product).FirstOrDefault(x => x.Type == ProductTypes.Proccesor);
            if (proccesor == null) return 0;

            var socketVendor = motherboard.Tags.Where(x => x.Name == "INTEL" || x.Name == "AMD").FirstOrDefault();
            if (!proccesor.Tags.Exists(x => x.Name == socketVendor.Name))
                return -1;

            var socket = motherboard.Tags.Where(x => x.Name.StartsWith("LGA")).FirstOrDefault();

            if (socket == null && proccesor.Tags.Exists(x => x.Name == "AMD"))
                return 1;

            return proccesor.Tags.Exists(x => x.Name == socket.Name) ? 1 : -1;
        }

        public int MotherboardRamCompatibility()
        {
            var motherboard = PcBuildProducts.Select(x => x.Product).FirstOrDefault(x => x.Type == ProductTypes.Motherboard);
            if (motherboard == null) return 0;

            var ram = PcBuildProducts.Select(x => x.Product).FirstOrDefault(x => x.Type == ProductTypes.RAM);
            if (ram == null) return 0;

            var motherboardRam = motherboard.Tags.Where(x => x.Name == "DDR3" || x.Name == "DDR4" || x.Name == "DDR5").FirstOrDefault();
            return ram.Tags.Exists(x => x.Name == motherboardRam.Name) ? 1 : -1;
        }

    }
}
