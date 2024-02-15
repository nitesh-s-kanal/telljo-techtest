using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Entity;
using Microsoft.AspNetCore.Authorization;

namespace ReadLater5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksApiController : ControllerBase
    {
        private readonly ReadLaterDataContext _context;

        public BookmarksApiController(ReadLaterDataContext context)
        {
            _context = context;
        }

        // GET: api/BookmarksApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookmark>>> GetBookmark()
        {
            return await _context.Bookmark.ToListAsync();
        }

        // GET: api/BookmarksApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bookmark>> GetBookmark(int id)
        {
            var bookmark = await _context.Bookmark.FindAsync(id);

            if (bookmark == null)
            {
                return NotFound();
            }

            return bookmark;
        }

        // PUT: api/BookmarksApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutBookmark(int id, Bookmark bookmark)
        {
            if (id != bookmark.ID)
            {
                return BadRequest();
            }

            _context.Entry(bookmark).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookmarkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BookmarksApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bookmark>> PostBookmark(Bookmark bookmark)
        {
            bookmark.CreateDate = DateTime.UtcNow;
            _context.Bookmark.Add(bookmark);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookmark", new { id = bookmark.ID }, bookmark);
        }

        // DELETE: api/BookmarksApi/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            var bookmark = await _context.Bookmark.FindAsync(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            _context.Bookmark.Remove(bookmark);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookmarkExists(int id)
        {
            return _context.Bookmark.Any(e => e.ID == id);
        }
    }
}
