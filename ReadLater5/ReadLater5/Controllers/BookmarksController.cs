using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Entity;

namespace ReadLater5.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly ReadLaterDataContext _context;

        public BookmarksController(ReadLaterDataContext context)
        {
            _context = context;
        }

        // GET: Bookmarks
        public async Task<IActionResult> Index()
        {
            var readLaterDataContext = _context.Bookmark.Include(b => b.Category);
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
            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

        // GET: Bookmarks/Create
        public IActionResult Create()
        {
            ViewData["Category"] = new SelectList(_context.Categories, "ID", "Name");
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
            ViewData["Category"] = new SelectList(_context.Categories, "ID", "Name", bookmark.CategoryId);
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
