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
        private static Dictionary<string, List<UserQuiz>> _userQuizes = new Dictionary<string, List<UserQuiz>>();

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
            foreach (var quiz in quizzes)
            {
                var percentage = quiz.Course.UserCourseResults
                    .Where(x => x.Username == user.Id)
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

        // POST: Quiz/StartQuiz
        [HttpPost]
        public async Task<IActionResult> StartQuiz(Guid id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var quiz = await _context.Quizes
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (quiz == null || user == null) return NotFound();

            if (!_userQuizes.ContainsKey(user.UserName))
            {
                _userQuizes.Add(user.UserName, new List<UserQuiz>());
            }

            var userQuiz = new UserQuiz(user, quiz);
            if (_userQuizes[user.UserName].Exists(x => x.Quiz.Id == id))
                userQuiz = _userQuizes[user.UserName].First(x => x.Quiz.Id == id);
            else
                _userQuizes[user.UserName].Add(userQuiz);

            return Redirect($"/Quiz/GetQuestion/{quiz.Id}?questionNum=0");
        }

        // GET: /Quiz/GetQuestion/5?questionNum=1
        public async Task<IActionResult> GetQuestion(Guid? id, int questionNum)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            if (user == null) return null;

            var quizesList = _userQuizes[user.UserName];
            if (quizesList == null) return NotFound();

            var userQuiz = quizesList.FirstOrDefault(x => x.Quiz.Id == id);
            if (userQuiz == null) return NotFound();

            if (questionNum < 0 || questionNum > userQuiz.Quiz.Questions.Count - 1)
                return BadRequest();

            var question = userQuiz.Quiz.Questions[questionNum];
            question.Answers = _context.Answers.Where(x => x.QuestionId == question.Id).ToList();
            //var userAnswer = userQuiz.QuestionAnswer[question.Id];
            if (userQuiz.QuestionAnswer.ContainsKey(question.Id))
                ViewData["userAnswer"] = userQuiz.QuestionAnswer[question.Id];

            ViewData["questionNum"] = questionNum;
            ViewData["questions"] = userQuiz.Quiz.Questions;

            return View(question);
        }

        // POST: /Quiz/AnswerQuestion/5
        [HttpPost]
        public async Task<IActionResult> AnswerQuestion(Guid id, Guid? answerId, int questionNum)
        {
            if (id == null) return NotFound();

            if (answerId == null)
                return Redirect($"/Quiz/GetQuestion/{id}?questionNum={questionNum}");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var answer = await _context.Answers
                .Include(x => x.Quiestion)
                    .ThenInclude(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == answerId);
            if (user == null || answer == null) return NotFound();

            var userQuiz = _userQuizes[user.UserName].FirstOrDefault(x => x.Quiz.Id == id);
            if (userQuiz == null) return NotFound();

            if (userQuiz.QuestionAnswer.ContainsKey(answer.Quiestion.Id))
                userQuiz.QuestionAnswer[answer.Quiestion.Id] = answer;
            else
                userQuiz.QuestionAnswer.Add(answer.Quiestion.Id, answer);

            int nextQuestion = questionNum + 1;
            if (nextQuestion > userQuiz.Quiz.Questions.Count - 1)
                nextQuestion = 0;

            return Redirect($"/Quiz/GetQuestion/{answer.Quiestion.QuizId}?questionNum={nextQuestion}");
        }

        // POST: /Quiz/FinishQuiz/5
        [HttpPost]
        public async Task<IActionResult> FinishQuiz(Guid? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var userQuiz = _userQuizes[user.UserName].FirstOrDefault(x => x.Quiz.Id == id);
            if (user == null || userQuiz == null)
                return NotFound();

            int quizQuestionsCount = userQuiz.Quiz.Questions.Count;
            int correctAnswers = userQuiz.QuestionAnswer.Values.Count(x => x.IsRight);
            double percentage = (correctAnswers / (double)quizQuestionsCount) * 100.0;

            var userCourseResult = await _context.UserCourseResults
                .Include(x => x.Course)
                    .ThenInclude(x => x.Quiz)
                .FirstOrDefaultAsync(x => x.Username == user.UserName
                || x.Course.Quiz.Id == id);

            if (userCourseResult != null)
            {
                if (percentage > userCourseResult.Percentage)
                    userCourseResult.Percentage = percentage;
                _context.Update(userCourseResult);
            }
            else
            {
                userCourseResult = new UserCourseResult();
                userCourseResult.Id = Guid.NewGuid();
                userCourseResult.Username = user.UserName;
                userCourseResult.Percentage = percentage;
                userCourseResult.CourseId = userQuiz.Quiz.CourseId;
            }

            await _context.SaveChangesAsync();

            _userQuizes[user.UserName] = _userQuizes[user.UserName]
                .Where(x => x.Quiz.Id != id)
                .ToList();

            return Redirect("/Course");
        }
    }
}
