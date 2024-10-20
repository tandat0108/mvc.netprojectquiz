﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Import namespace cho [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;
using ProjectQuiz.Models; // Đảm bảo thêm namespace cho Models

namespace ProjectQuiz.Controllers
{
    [Authorize] // Áp dụng [Authorize] cho tất cả các hành động trong controller này
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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Chuyển đổi UserId từ string sang int
            var projects = _context.Projects
                .Include(p => p.User)
                .Where(p => p.UserId == userId); // Chỉ lấy các dự án của người dùng đã đăng nhập

            return View(await projects.ToListAsync());
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

            // Lưu ProjectId vào TempData
            TempData["CurrentProjectId"] = id;

            // Lấy danh sách thành viên của dự án
            var members = await _context.ProjectMembers
                .Include(pm => pm.User) // Lấy thông tin người dùng
                .Where(pm => pm.ProjectId == id)
                .ToListAsync();

            ViewBag.Members = members; // Gán danh sách thành viên vào ViewBag để sử dụng trong view

            return View(project); // Trả về view details
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
                // Gán UserId cho dự án mới
                project.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddMember(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }

            var users = _context.Users.ToList();
            ViewBag.Users = users; // Lấy danh sách người dùng

            return View(project); // Trả về view AddMember.cshtml
        }



        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
