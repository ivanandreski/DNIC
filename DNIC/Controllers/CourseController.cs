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
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DNIC.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Course
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(username));
            if (user == null)
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            var userCourseResults = await _context.UserCourseResults
                .Include(x => x.User)
                .Include(x => x.Course)
                .Where(x => x.Username.Equals(user.Id))
                .ToListAsync();

            if (userCourseResults.Count < _context.Courses.Count())
            {
                foreach(var course in _context.Courses)
                {
                    var userCourseResult = new UserCourseResult();
                    userCourseResult.Percentage = 0;
                    userCourseResult.CourseId = course.Id;
                    userCourseResult.Course = course;
                    userCourseResult.Username = user.UserName;
                    userCourseResult.User = user;

                    _context.UserCourseResults.Add(userCourseResult);
                }

                await _context.SaveChangesAsync();
            }

            return View(userCourseResults);
        }

        // GET: Course/Learn/5
        public async Task<IActionResult> Learn(Guid? id, int section = 1)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(x => x.Sections)
                .Include(x => x.Quiz)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            course.Sections = course.Sections.OrderBy(x => x.Page).ToList();

            if (course.Sections.Count > 0)
            {
                int prevPage = -1;
                int nextPage = -1;
                Section currentSec = null;

                for (int i = 0; i < course.Sections.Count; i++)
                {
                    if (course.Sections[i].Page == section)
                    {
                        prevPage = i == 0 ? -1 : course.Sections[i - 1].Page;
                        nextPage = i + 1 == course.Sections.Count ? -1 : course.Sections[i + 1].Page;
                        currentSec = course.Sections[i];

                        break;
                    }
                }

                if (currentSec == null)
                {
                    return NotFound();
                }

                ViewData["nextPage"] = nextPage;
                ViewData["prevPage"] = prevPage;
                ViewData["course"] = course;
                double progress = (section / (double)course.Sections.Count) * 100.0;
                ViewData["progress"] = progress;

                return View(currentSec);
            }

            return Redirect("/Course");
        }

        // GET: Course/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(x => x.Sections)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            course.Sections = course.Sections.OrderBy(x => x.Page).ToList();

            return View(course);
        }

        // GET: Course/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CourseDto courseDto)
        {
            if (ModelState.IsValid)
            {
                var course = new Course();
                course.Id = Guid.NewGuid();
                course.Name = courseDto.Name;
                course.Description = courseDto.Description;

                using (MemoryStream ms = new MemoryStream())
                {
                    courseDto.Image.CopyTo(ms);
                    course.Image = ms.ToArray();
                }

                _context.Courses.Add(course);

                // Quiz for course
                var quiz = new Quiz();
                quiz.Id = Guid.NewGuid();
                quiz.Course = course;
                quiz.CourseId = course.Id;
                _context.Quizes.Add(quiz);

                // Results for users
                foreach (var user in _context.Users)
                {
                    var userCourseResult = new UserCourseResult();
                    userCourseResult.Percentage = 0;
                    userCourseResult.CourseId = course.Id;
                    userCourseResult.Course = course;
                    userCourseResult.Username = user.UserName;
                    userCourseResult.User = user;

                    _context.UserCourseResults.Add(userCourseResult);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(courseDto);
        }

        // GET: Course/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var dto = new CourseDto();
            dto.Description = course.Description;
            dto.Name = course.Name;

            var stream = new MemoryStream(course.Image);
            dto.Image = new FormFile(stream, 0, course.Image.Length, "image", "fileName");

            dto.ImageArray = course.Image;
            dto.CourseId = course.Id;

            return View(dto);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, CourseDto dto)
        {
            if (ModelState.IsValid)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);
                if (course == null)
                    return NotFound();
                course.Description = dto.Description;
                course.Name = dto.Name;

                using (MemoryStream ms = new MemoryStream())
                {
                    dto.Image.CopyTo(ms);
                    course.Image = ms.ToArray();
                }

                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: Course/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(Guid id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
