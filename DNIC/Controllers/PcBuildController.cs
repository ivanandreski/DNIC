using DNIC.Data;
using DNIC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DNIC.Data;
using DNIC.Models;
using Microsoft.AspNetCore.Authorization;

namespace DNIC.Controllers
{
    [Authorize]
    public class PcBuildController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly User user;

        public PcBuildController(ApplicationDbContext context)
        {
            _context = context;

            // TODO: see if this works
            user = _context.Users
                .Include(x => x.PcBuild)
                    .ThenInclude(x => x.PcBuildProducts)
                        .ThenInclude(x => x.Product)
                .First(x => x.UserName == User.Identity.Name);
        }

        public async Task<IActionResult> Index()
        {
            return View(user.PcBuild);
        }

        [HttpPost]
        public async Task<IActionResult> SetProduct(Guid? productId)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == productId);
            if (product == null) return NotFound();

            user.PcBuild.PcBuildProducts = user.PcBuild.PcBuildProducts
                 .Where(x => x.Product.Type != product.Type)
                 .ToList();
            _context.PcBuildProducts.Add(new PcBuildProduct(product.Id, user.PcBuild.Id));
            await _context.SaveChangesAsync();

            return Redirect("/PcBuild");
        }
    }
}
