using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Import namespace for [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;

namespace ProjectQuiz.Controllers
{
    [Authorize] // Apply [Authorize] to all actions in this controller
    public class ProjectsController : Controller
    {
        private readonly QuizzDbContext _context;

        public ProjectsController(QuizzDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve role from Claims
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var isAdmin = userRole == "admin"; // Compare with the "admin" string

            // Get the list of projects created by the user
            var ownerProjects = await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync();

            // Get the list of projects the user is a member of through ProjectMembers
            var memberProjects = await _context.ProjectMembers
                .Include(pm => pm.Project)
                .Where(pm => pm.UserId == userId)
                .Select(pm => pm.Project)
                .ToListAsync();

            ViewBag.OwnerProjects = ownerProjects;
            ViewBag.MemberProjects = memberProjects;
            ViewBag.IsAdmin = isAdmin; // Pass isAdmin flag to ViewBag to use in the view

            return View();
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            // Store ProjectId in TempData
            TempData["CurrentProjectId"] = id;

            // Get the list of members in the project
            var members = await _context.ProjectMembers
                .Include(pm => pm.User) // Include user information
                .Where(pm => pm.ProjectId == id)
                .ToListAsync();

            ViewBag.Members = members; // Assign the list of members to ViewBag to use in the view

            return View(project); // Return details view
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IntroductionVideoUrl,CreatedDate,LastUpdatedDate,Status")] Project project)
        {
            if (ModelState.IsValid)
            {
                // Assign UserId to the new project
                project.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                project.CreatedDate = DateTime.Now; // Set project creation date
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IntroductionVideoUrl,CreatedDate,LastUpdatedDate,Status")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find Project and include related Quizzes and QuizResults
            var project = await _context.Projects
                .Include(p => p.Quizzes)
                    .ThenInclude(q => q.QuizResults) // Include QuizResults related to Quizzes
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project != null)
            {
                // Delete all QuizResults related to each Quiz of the Project
                foreach (var quiz in project.Quizzes)
                {
                    _context.QuizResults.RemoveRange(quiz.QuizResults);
                }

                // Delete all Quizzes related to the Project
                _context.Quizzes.RemoveRange(project.Quizzes);

                // Delete Project
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
