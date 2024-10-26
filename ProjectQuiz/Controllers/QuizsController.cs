using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;
using ProjectQuiz.ViewModels;

namespace ProjectQuiz.Controllers
{
    public class QuizsController : Controller
    {
        private readonly QuizzDbContext _context;

        public QuizsController(QuizzDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            ViewBag.ProjectId = id;
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.ProjectOwnerId = project.UserId;

            var quizzes = await _context.Quizzes
                .Where(q => q.ProjectId == id)
                .ToListAsync();

            return View(quizzes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Project)
                .FirstOrDefaultAsync(m => m.QuizId == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpGet]
        public IActionResult Create(int projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuizId,Title,ProjectId,QuestionType,QuestionText")] Quiz quiz, string[] Answers, string correctAnswerMCQ, string CorrectAnswerSAQ, int projectId)
        {
            if (quiz.ProjectId == 0)
            {
                quiz.ProjectId = projectId;
            }

            Console.WriteLine($"QuestionType: {quiz.QuestionType}");
            Console.WriteLine($"Answers Input: {string.Join(", ", Answers)}");
            Console.WriteLine($"CorrectAnswerMCQ Input: {correctAnswerMCQ}");
            Console.WriteLine($"CorrectAnswerSAQ Input: {CorrectAnswerSAQ}");

            if (true)
            {
                if (quiz.QuestionType == "multiple-choice")
                {
                    quiz.Answers = string.Join(",", Answers);
                    quiz.IsCorrect = correctAnswerMCQ;
                    quiz.CorrectAnswer = null;
                }
                else if (quiz.QuestionType == "short-answer")
                {
                    if (string.IsNullOrWhiteSpace(CorrectAnswerSAQ))
                    {
                        ModelState.AddModelError("CorrectAnswerSAQ", "Correct answer cannot be empty.");
                        return View(quiz);
                    }
                    quiz.CorrectAnswer = CorrectAnswerSAQ;
                    quiz.IsCorrect = null;
                }

                try
                {
                    _context.Add(quiz);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Quiz saved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving quiz: {ex.Message}");
                    return View(quiz);
                }

                return RedirectToAction(nameof(Index), new { id = quiz.ProjectId });
            }

            ViewBag.ProjectId = quiz.ProjectId;
            return View(quiz);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Id", quiz.ProjectId);
            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuizId,Title,ProjectId,QuestionType,QuestionText,CorrectAnswer,Answers")] Quiz quiz, string[] Answers)
        {
            if (id != quiz.QuizId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (quiz.QuestionType == "multiple-choice")
                    {
                        quiz.Answers = string.Join(",", Answers);
                    }
                    else if (quiz.QuestionType == "short-answer")
                    {
                        quiz.Answers = null;
                        quiz.CorrectAnswer = quiz.CorrectAnswer;
                    }

                    _context.Update(quiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.QuizId))
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

            ViewBag.ProjectId = quiz.ProjectId;
            return View(quiz);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Project)
                .FirstOrDefaultAsync(m => m.QuizId == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuiz(int id, int projectId)
        {
            var quiz = await _context.Quizzes.FindAsync(id);

            if (quiz != null)
            {
                var quizResults = _context.QuizResults.Where(qr => qr.QuizId == id);
                _context.QuizResults.RemoveRange(quizResults);

                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();

                TempData["DeleteMessage"] = $"Quiz '{quiz.Title}' has been successfully deleted.";
            }

            return RedirectToAction("Index", new { id = projectId });
        }

        [HttpGet]
        public async Task<IActionResult> TakeAllQuizzes(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var hasResults = await _context.QuizResults
                .Where(qr => qr.Quiz.ProjectId == id && qr.UserId == userId)
                .AnyAsync();

            if (hasResults)
            {
                TempData["RetakeConfirmation"] = "You have already completed this quiz.";
            }

            var quizzes = await _context.Quizzes
                .Where(q => q.ProjectId == id)
                .ToListAsync();

            ViewBag.ProjectId = id;
            return View("TakeAllQuizzes", quizzes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAllQuizzes(Dictionary<int, string> selectedAnswers)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            decimal totalScore = 0;
            var results = new List<QuizResult>();

            foreach (var quizId in selectedAnswers.Keys)
            {
                var quiz = await _context.Quizzes.AsNoTracking().FirstOrDefaultAsync(q => q.QuizId == quizId);
                if (quiz == null)
                {
                    return NotFound();
                }

                var selectedAnswer = selectedAnswers[quizId];
                bool isCorrect = false;

                if (quiz.QuestionType == "multiple-choice" && int.TryParse(selectedAnswer, out int answerIndex))
                {
                    selectedAnswer = (answerIndex + 1).ToString();
                    isCorrect = selectedAnswer == quiz.IsCorrect;
                }
                else if (quiz.QuestionType == "short-answer")
                {
                    isCorrect = selectedAnswer.Trim().Equals(quiz.CorrectAnswer?.Trim(), StringComparison.OrdinalIgnoreCase);
                }

                totalScore += isCorrect ? 10 : 0;

                var result = new QuizResult
                {
                    UserId = userId,
                    QuizId = quiz.QuizId,
                    Score = isCorrect ? 10 : 0,
                    SelectedAnswer = selectedAnswer,
                    Comment = isCorrect ? "Correct" : "Incorrect"
                };

                results.Add(result);
            }

            var quizIds = selectedAnswers.Keys.ToList();
            var oldResults = _context.QuizResults.Where(qr => qr.UserId == userId && quizIds.Contains(qr.QuizId)).ToList();
            _context.QuizResults.RemoveRange(oldResults);
            await _context.QuizResults.AddRangeAsync(results);
            await _context.SaveChangesAsync();

            TempData["QuizCompleted"] = "You have completed the quiz.";
            var firstQuiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.QuizId == quizIds.First());
            return firstQuiz != null
                ? RedirectToAction("Details", "Projects", new { id = firstQuiz.ProjectId })
                : RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRetake(int id)
        {
            TempData.Remove("RetakeConfirmation");
            return RedirectToAction("TakeAllQuizzes", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> QuizResults(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            bool isAdmin = project.UserId == userId;

            if (isAdmin)
            {
                var userScores = await _context.QuizResults
                    .Include(qr => qr.User)
                    .Where(qr => qr.Quiz.ProjectId == id)
                    .GroupBy(qr => qr.User)
                    .Select(g => new
                    {
                        User = g.Key,
                        TotalScore = g.Sum(qr => qr.Score)
                    })
                    .ToListAsync();

                ViewBag.UserScores = userScores;
                ViewBag.IsAdmin = true;
            }
            else
            {
                var results = await _context.QuizResults
                    .Where(qr => qr.UserId == userId && qr.Quiz.ProjectId == id)
                    .Include(qr => qr.Quiz)
                    .ToListAsync();

                if (!results.Any())
                {
                    ViewBag.NoResults = "You haven't taken the quiz yet.";
                }
                else
                {
                    ViewBag.TotalScore = results.Sum(qr => qr.Score);
                    ViewBag.UserResults = results.Select(qr => new QuizResultViewModel
                    {
                        QuizResult = qr,
                        SelectedAnswer = qr.SelectedAnswer
                    }).ToList();
                }

                ViewBag.IsAdmin = false;
            }

            return View();
        }

        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.QuizId == id);
        }
    }
}
