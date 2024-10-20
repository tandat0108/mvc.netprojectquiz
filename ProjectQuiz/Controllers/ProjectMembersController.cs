using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;

namespace ProjectQuiz.Controllers
{
    public class ProjectMembersController : Controller
    {
        private readonly QuizzDbContext _context;

        public ProjectMembersController(QuizzDbContext context)
        {
            _context = context;
        }

        // GET: ProjectMembers
        public async Task<IActionResult> Index(int projectId)
        {
            ViewBag.ProjectId = projectId; // Gán ProjectId từ URL vào ViewBag
            var members = await _context.ProjectMembers
                .Include(pm => pm.User)
                .Where(pm => pm.ProjectId == projectId)  // Lọc theo ProjectId
                .ToListAsync();

            return View(members);
        }


        // GET: ProjectMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectMember = await _context.ProjectMembers
                .Include(p => p.Project)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectMember == null)
            {
                return NotFound();
            }

            return View(projectMember);
        }

        // GET: ProjectMembers/Create
        public IActionResult Create(int projectId)
        {
            ViewBag.ProjectId = projectId; // Gán ProjectId từ URL vào ViewBag để sử dụng trong view
            ViewBag.Users = new SelectList(_context.Users, "Id", "Username"); // Lấy danh sách người dùng
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Role")] ProjectMember projectMember, int projectId)
        {
            projectMember.ProjectId = projectId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.ProjectMembers.Add(projectMember);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { projectId = projectId });
                }
                catch (Exception ex)
                {
                    // Log chi tiết của lỗi
                    var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError("", $"Có lỗi xảy ra khi lưu dữ liệu: {innerException}");
                }
            }

            ViewBag.ProjectId = projectId;
            ViewBag.Users = new SelectList(_context.Users, "Id", "Username", projectMember.UserId);
            return View(projectMember);
        }



        // GET: ProjectMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectMember = await _context.ProjectMembers.FindAsync(id);
            if (projectMember == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", projectMember.ProjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", projectMember.UserId);
            return View(projectMember);
        }

        // POST: ProjectMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,UserId,Role")] ProjectMember projectMember)
        {
            if (id != projectMember.Id)
            {
                return NotFound();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", projectMember.ProjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", projectMember.UserId);
            return View(projectMember);
        }

        // GET: ProjectMembers/Delete/5
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
            if (projectMember == null)
            {
                return NotFound();
            }

            return View(projectMember);
        }

        // POST: ProjectMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectMember = await _context.ProjectMembers.FindAsync(id);
            if (projectMember != null)
            {
                _context.ProjectMembers.Remove(projectMember);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectMemberExists(int id)
        {
            return _context.ProjectMembers.Any(e => e.Id == id);
        }
    }
}
