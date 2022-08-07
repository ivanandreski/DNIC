using DNIC.Data;
using DNIC.Models;
using DNIC.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace DNIC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private static string anhochImageUrl = "https://devicenetwork.s3.eu-west-2.amazonaws.com/images/logo/uur0uap.png";
        private static string setecImageUrl = "https://www.setec.mk/image/catalog/Promo/setec_logo_modal.jpg";
        private static string ddImageUrl = "https://ddcom.mk/wp-content/uploads/2022/04/ddstore_logo.png";

        private static readonly Dictionary<string, string> images = new Dictionary<string, string>()
        {
            ["Anhoch"] = anhochImageUrl,
            ["Setec"] = setecImageUrl,
            ["DdStore"] = ddImageUrl
        };

        private readonly ApplicationDbContext _context;

        public ProductApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile([FromForm] ProductDto dto)
        {
            var product = new Product();
            product.Id = Guid.NewGuid();
            product.Name = dto.Name;
            product.Url = dto.Url;
            product.Price = dto.Price;
            product.Type = dto.Type;
            product.RealId = dto.RealId;
            product.Tags = getTags(product);

            Random random = new Random();
            var storeImageKey = random.Next(0, images.Count);
            var storeName = images.Keys.ToArray()[storeImageKey];
            var storeImageUrl = images[storeName];

            product.StoreName = storeName;
            product.StoreImageUrl = storeImageUrl;

            var existingProduct = _context.Products.FirstOrDefault(x => x.RealId == dto.RealId);
            if (existingProduct != null)
                _context.Remove(existingProduct);

            _context.Products.Add(product);

            return Ok(await _context.SaveChangesAsync());
        }
        private List<Tag> getTags(Product product)
        {
            switch (product.Type)
            {
                case "Motherboard":
                    return getMotherboardTags(product);
                default:
                    return new List<Tag>();
            }
        }

        private List<Tag> getMotherboardTags(Product product)
        {
            string name = product.Name;
            Guid productId = product.Id;

            List<Tag> tags = new List<Tag>();
            if (name.Contains("LGA"))
                tags.Add(new Tag("INTEL", productId));
            else
                tags.Add(new Tag("AMD", productId));

            foreach (var split in name.Split("\\s+"))
            {
                if (split.Contains("DDR"))
                {
                    tags.Add(new Tag(split, productId));
                    break;
                }
            }

            return tags;
        }
    }
}
