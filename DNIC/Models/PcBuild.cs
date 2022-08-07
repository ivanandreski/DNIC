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

        //public int MotherboardProccesorCompatibility()
        //{
        //    var motherboard = Products.FirstOrDefault(x => x.Type == ProductTypes.Motherboard);
        //    if (motherboard == null) return -1;

        //    var proccesor = Products.FirstOrDefault(x => x.Type == ProductTypes.Proccesor);
        //    if (proccesor == null) return -1;

        //    var socketVendor = motherboard.Tags.Where(x => x.Name == "INTEL" || x.Name == "AMD").FirstOrDefault();
        //    return proccesor.Tags.Exists(x => x.Name == socketVendor.Name) ? 1 : -1;
        //}

        //public int MotherboardRamCompatibility()
        //{
        //    var motherboard = Products.FirstOrDefault(x => x.Type == ProductTypes.Motherboard);
        //    if (motherboard == null) return -1;

        //    var ram = Products.FirstOrDefault(x => x.Type == ProductTypes.RAM);
        //    if (ram == null) return -1;

        //    var motherboardRam = motherboard.Tags.Where(x => x.Name == "DDR3" || x.Name == "DDR4" || x.Name == "DDR5").FirstOrDefault();
        //    return ram.Tags.Exists(x => x.Name == motherboardRam.Name) ? 1 : -1;
        //}

        //public int PowerSupplyGraphicsCardCompatibility()
        //{
        //    // TODO: watts needed

        //    return 0;
        //}

        //public int CaseGraphicsCardCompatibility()
        //{
        //    // TODO: in milimeters

        //    return 0;
        //}

    }
}
