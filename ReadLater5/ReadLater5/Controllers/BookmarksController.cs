using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Entity;
using Microsoft.AspNetCore.Identity;

namespace ReadLater5.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly ReadLaterDataContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public BookmarksController(ReadLaterDataContext context, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: Bookmarks
        public async Task<IActionResult> Index(int id)
        {
            var readLaterDataContext = _context.Bookmark.Where(b => b.CategoryId == id).Include(b => b.Category);
            ViewData["CategoryId"] = id;
            return View(await readLaterDataContext.ToListAsync());
        }

        // GET: Bookmarks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmark = await _context.Bookmark
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.ID == id);

            ViewData["CategoryId"] = bookmark.CategoryId;

            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

        // GET: Bookmarks/Create
        public IActionResult Create(int id)
        {
            string userId = _signInManager.UserManager.GetUserId(User);
            ViewData["Category"] = new SelectList(_context.Categories.Where(c => c.UserId == userId), "ID", "Name", id);
            ViewData["CategoryId"] = id;
            return View();
        }

        // GET: Bookmarks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmark = await _context.Bookmark.FindAsync(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            string userId = _signInManager.UserManager.GetUserId(User);
            ViewData["Category"] = new SelectList(_context.Categories.Where(c => c.UserId == userId), "ID", "Name", bookmark.CategoryId);
            ViewData["CategoryId"] = bookmark.CategoryId;
            return View(bookmark);
        }

        // GET: Bookmarks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmark = await _context.Bookmark
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

        private bool BookmarkExists(int id)
        {
            return _context.Bookmark.Any(e => e.ID == id);
        }
    }
}
