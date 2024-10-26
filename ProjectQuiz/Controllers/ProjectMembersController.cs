using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;
using System.Security.Claims;

namespace ProjectQuiz.Controllers
{
    [Authorize]
    public class ProjectMembersController : Controller
    {
        private readonly QuizzDbContext _context;

        public ProjectMembersController(QuizzDbContext context)
        {
            _context = context;
        }

        private async Task<bool> IsProjectOwner(int projectId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var project = await _context.Projects.FindAsync(projectId);
            return project != null && project.UserId == userId;
        }

        public async Task<IActionResult> Index(int projectId)
        {
            var isOwner = await IsProjectOwner(projectId);
            ViewBag.IsProjectOwner = isOwner;
            ViewBag.ProjectId = projectId;

            var members = await _context.ProjectMembers
                .Include(pm => pm.User)
                .Where(pm => pm.ProjectId == projectId)
                .ToListAsync();

            return View(members);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Create(int projectId)
        {
            if (!await IsProjectOwner(projectId))
            {
                return Forbid();
            }

            ViewBag.ProjectId = projectId;
            ViewBag.Users = new SelectList(_context.Users, "Id", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("UserId,Role")] ProjectMember projectMember, int projectId)
        {
            if (!await IsProjectOwner(projectId))
            {
                return Forbid();
            }

            projectMember.ProjectId = projectId;
            var isExists = _context.ProjectMembers.Any(m => m.UserId == projectMember.UserId && m.ProjectId == projectId);

            if (!isExists)
            {
                try
                {
                    _context.ProjectMembers.Add(projectMember);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { projectId = projectId });
                }
                catch (Exception ex)
                {
                    var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData["ErrorMessage"] = $"An error occurred while saving data: {innerException}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "This member already exists in the project.";
            }

            ViewBag.ProjectId = projectId;
            ViewBag.Users = new SelectList(_context.Users, "Id", "Username", projectMember.UserId);
            return View(projectMember);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectMember = await _context.ProjectMembers.FindAsync(id);
            if (projectMember == null || !await IsProjectOwner(projectMember.ProjectId))
            {
                return Forbid();
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", projectMember.ProjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", projectMember.UserId);
            return View(projectMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,UserId,Role")] ProjectMember projectMember)
        {
            if (id != projectMember.Id || !await IsProjectOwner(projectMember.ProjectId))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectMemberExists(projectMember.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { projectId = projectMember.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", projectMember.ProjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", projectMember.UserId);
            return View(projectMember);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectMember = await _context.ProjectMembers
                .Include(p => p.Project)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectMember == null || !await IsProjectOwner(projectMember.ProjectId))
            {
                return Forbid();
            }

            return View(projectMember);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int projectId)
        {
            if (!await IsProjectOwner(projectId))
            {
                return Forbid();
            }

            var member = await _context.ProjectMembers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member != null)
            {
                string username = member.User.Username;
                _context.ProjectMembers.Remove(member);
                await _context.SaveChangesAsync();
                TempData["DeleteMessage"] = $"Member '{username}' has been successfully removed from the project.";
            }

            return RedirectToAction("Index", new { projectId = projectId });
        }

        private bool ProjectMemberExists(int id)
        {
            return _context.ProjectMembers.Any(e => e.Id == id);
        }
    }
}
