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
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Quiz
        public async Task<IActionResult> Index()
        {
            var quizzes = await _context
                .Quizes
                .Include(q => q.Course)
                    .ThenInclude(c => c.UserCourseResults)
                .ToListAsync();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            if (user == null) return NotFound();

            List<double> percentages = new List<double>();
            foreach(var quiz in quizzes)
            {
                var percentage = _context.UserCourseResults
                    .Where(x => x.CourseId == quiz.CourseId && x.Username == user.UserName)
                    .Select(x => x.Percentage)
                    .FirstOrDefault();

                percentages.Add(percentage);
            }

            ViewData["percentages"] = percentages;

            return View(quizzes);
        }

        // GET: Quiz/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizes
                .Include(q => q.Course)
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        private bool QuizExists(Guid id)
        {
            return _context.Quizes.Any(e => e.Id == id);
        }
    }
}
