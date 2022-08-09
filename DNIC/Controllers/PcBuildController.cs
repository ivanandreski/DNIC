using DNIC.Data;
using DNIC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DNIC.Controllers
{
    [Authorize]
    public class PcBuildController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PcBuildController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            if (user == null) return NotFound();

            var pcBuild = await _context.PcBuild
                .Include(x => x.PcBuildProducts)
                    .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.Tags)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if(pcBuild == null)
            {
                pcBuild = new PcBuild();
                pcBuild.Id = Guid.NewGuid();
                pcBuild.UserId = user.Id;

                pcBuild = _context.PcBuild.Add(pcBuild).Entity;
                await _context.SaveChangesAsync();
            }

            Dictionary<string, List<Product>> productsDictionary = new Dictionary<string, List<Product>>();

            Dictionary<string, Product> productsPcBuildDictionary = new Dictionary<string, Product>()
            {
                [ProductTypes.Motherboard] = getProduct(pcBuild, ProductTypes.Motherboard),
                [ProductTypes.Proccesor] = getProduct(pcBuild, ProductTypes.Proccesor),
                [ProductTypes.GraphicsCard] = getProduct(pcBuild, ProductTypes.GraphicsCard),
                [ProductTypes.HardDrive] = getProduct(pcBuild, ProductTypes.HardDrive),
                [ProductTypes.SolidStateDrive] = getProduct(pcBuild, ProductTypes.SolidStateDrive),
                [ProductTypes.RAM] = getProduct(pcBuild, ProductTypes.RAM),
                [ProductTypes.Case] = getProduct(pcBuild, ProductTypes.Case),
            };

            var price = productsPcBuildDictionary.Values
                .Select(x => x?.Price ?? 0)
                .Sum();

            ViewData["price"] = price;

            ViewData["ProductsPcBuild"] = productsPcBuildDictionary;

            foreach(var key in productsPcBuildDictionary.Keys)
            {
                productsDictionary[key] = _context.Products
                    .Where(x => x.Type == key)
                    .ToList();
            }

            ViewData["Products"] = productsDictionary;

            return View(pcBuild);
        }

        [HttpPost]
        public async Task<IActionResult> SetProduct(Guid? productId)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == productId);
            if (product == null) return Redirect("/PcBuild");

            var user = await _context.Users
                .Include(x => x.PcBuild)
                    .ThenInclude(x => x.PcBuildProducts)
                        .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            if (user == null) return NotFound();

            user.PcBuild.PcBuildProducts = user.PcBuild.PcBuildProducts
                 .Where(x => x.Product.Type != product.Type)
                 .ToList();
            _context.PcBuildProducts.Add(new PcBuildProduct(product.Id, user.PcBuild.Id));
            await _context.SaveChangesAsync();

            return Redirect("/PcBuild");
        }

        private Product getProduct(PcBuild pcBuild, string type)
        {
            return _context.PcBuildProducts
                .Include(x => x.Product)
                .Where(x => x.PcBuildId == pcBuild.Id && x.Product.Type == type)
                .Select(x => x.Product)
                .FirstOrDefault();
        }
    }
}
