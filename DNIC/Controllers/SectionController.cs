using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DNIC.Data;
using DNIC.Models;
using DNIC.Models.Dto;

namespace DNIC.Controllers
{
    public class SectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Section
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sections.ToListAsync());
        }

        // GET: Section/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        // GET: Section/Create
        public IActionResult Create(Guid? courseId)
        {
            if(courseId == null)
                return View("Error");

            var dto = new SectionDto();
            dto.CourseId = courseId.Value;

            return View();
        }

        // POST: Section/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SectionDto sectionDto)
        {
            if (ModelState.IsValid)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id.Equals(sectionDto.CourseId));
                if (course == null)
                    return View(sectionDto);

                var section = new Section();

                section.Id = Guid.NewGuid();
                section.Title = sectionDto.Title;
                section.Text = sectionDto.Text;
                section.CourseId = sectionDto.CourseId;
                section.Course = course;

                section.Page = (await _context.Sections.Where(x => x.CourseId.Equals(sectionDto.CourseId)).CountAsync()) + 1;

                _context.Add(section);
                await _context.SaveChangesAsync();
                return Redirect($"/Course/Details/{sectionDto.CourseId}");
            }
            return View(sectionDto);
        }

        // GET: Section/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections.FindAsync(id);
            if (section == null)
            {
                return NotFound();
            }
            return View(section);
        }

        // POST: Section/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Page,Title,Text,CourseId,Id")] Section section)
        {
            if (id != section.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(section);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionExists(section.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect($"/Course/Details/{section.CourseId}");
            }
            return View(section);
        }

        // GET: Section/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        // POST: Section/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var section = await _context.Sections.FindAsync(id);
            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return Redirect($"/Course/Details/{section.CourseId}");
        }

        private bool SectionExists(Guid id)
        {
            return _context.Sections.Any(e => e.Id == id);
        }
    }
}
