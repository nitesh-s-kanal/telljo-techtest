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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;

namespace ReadLater5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksApiController : ControllerBase
    {
        private readonly ReadLaterDataContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public BookmarksApiController(ReadLaterDataContext context, IOptions<TokenValidationParameters> tokenValidationParameters)
        {
            _context = context;
            _tokenValidationParameters = tokenValidationParameters.Value;
        }

        // GET: api/BookmarksApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookmark>>> GetBookmark()
        {
            if (!IsTokenValid(HttpContext.Request.Headers["Authorization"]))
            {
                return Unauthorized("Invalid access token");
            }

            return await _context.Bookmark.ToListAsync();
        }

        // GET: api/BookmarksApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bookmark>> GetBookmark(int id)
        {
            if (!IsTokenValid(HttpContext.Request.Headers["Authorization"]))
            {
                return Unauthorized("Invalid access token");
            }

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
        public async Task<IActionResult> PutBookmark(int id, Bookmark bookmark)
        {
            if (!IsTokenValid(HttpContext.Request.Headers["Authorization"]))
            {
                return Unauthorized("Invalid access token");
            }

            if (id != bookmark.ID)
            {
                return BadRequest();
            }

            //var existingBookmark = await _context.Bookmark.FindAsync(id);
            _context.Entry(bookmark).State = EntityState.Modified;

            try
            {
               // bookmark.CreateDate = existingBookmark.CreateDate;
                bookmark.LastUpdatedDate = DateTime.UtcNow;
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
            if (!IsTokenValid(HttpContext.Request.Headers["Authorization"]))
            {
                return Unauthorized("Invalid access token");
            }

            bookmark.CreateDate = DateTime.UtcNow;
            _context.Bookmark.Add(bookmark);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookmark", new { id = bookmark.ID }, bookmark);
        }

        // DELETE: api/BookmarksApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            if (!IsTokenValid(HttpContext.Request.Headers["Authorization"]))
            {
                return Unauthorized("Invalid access token");
            }

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

        [NonAction]
        private bool IsTokenValid(string accessToken)
        {
            if(!accessToken.StartsWith("Bearer "))
            {
                return false;
            }

            //removing the Bearer token value.
            accessToken = accessToken.Replace("Bearer ","");

            var tokenHandler = new JwtSecurityTokenHandler();

            _tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7fcee9e2-e186-49cb-8ddd-784edde9aad2"));
            _tokenValidationParameters.ValidAudience = "test-audience";
            _tokenValidationParameters.ValidIssuer = "test-issuer";

            var token = tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var securityToken);

            var jwtToken = securityToken as JwtSecurityToken;

            if(jwtToken != null && jwtToken.ValidTo > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }
    }
}
