﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;

namespace ProjectQuiz.Controllers
{
    public class QuizsController : Controller
    {
        private readonly QuizzDbContext _context;

        public QuizsController(QuizzDbContext context)
        {
            _context = context;
        }

        // GET: Quizs
        public async Task<IActionResult> Index()
        {
            var quizzDbContext = _context.Quizzes.Include(q => q.Project);
            return View(await quizzDbContext.ToListAsync());
        }

        // GET: Quizs/Details/5
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

        // GET: Quizs/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id");
            return View();
        }

        // POST: Quizs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuizId,Title,ProjectId,QuestionType,QuestionText,IsCorrect,Answers")] Quiz quiz, string[] Answers)
        {
            if (ModelState.IsValid)
            {
                if (quiz.QuestionType == "multiple-choice")
                {
                    // Nối các câu trả lời của câu trắc nghiệm thành chuỗi cách nhau bằng dấu phẩy
                    quiz.Answers = string.Join(",", Answers);
                }
                else if (quiz.QuestionType == "true-false")
                {
                    // Xóa câu trả lời cho câu hỏi True/False vì không cần thiết
                    quiz.Answers = "";

                    // Giá trị IsCorrect đã được gán thông qua form (1 cho True, 0 cho False)
                    // Không cần thay đổi gì thêm
                }

                // Lưu vào cơ sở dữ liệu
                _context.Add(quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", quiz.ProjectId);
            return View(quiz);
        }

        // GET: Quizs/Edit/5
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", quiz.ProjectId);
            return View(quiz);
        }

        // POST: Quizs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuizId,Title,ProjectId,QuestionType,QuestionText,IsCorrect,Answers")] Quiz quiz, string[] Answers)
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
                        // Nối các câu trả lời của câu trắc nghiệm thành chuỗi cách nhau bằng dấu phẩy
                        quiz.Answers = string.Join(",", Answers);
                    }
                    else if (quiz.QuestionType == "true-false")
                    {
                        // Xóa câu trả lời cho câu hỏi True/False vì không cần thiết
                        quiz.Answers = "";
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", quiz.ProjectId);
            return View(quiz);
        }

        // GET: Quizs/Delete/5
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

        // POST: Quizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.QuizId == id);
        }
    }
}