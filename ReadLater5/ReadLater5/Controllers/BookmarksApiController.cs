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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Services;
using System.Data;

namespace ReadLater5.Controllers
{
    [Route("api/bookmarks")]
    [ApiController]
    public class BookmarksApiController : ControllerBase
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBookmarkService _bookmarkService;

        public BookmarksApiController(IBookmarkService bookmarkService, SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager)
        {
            _tokenValidationParameters = new();
            _signInManager = signInManager;
            _userManager = userManager;
            _bookmarkService = bookmarkService;
        }

        // GET: api/bookmarks/getallbookmarks
        [HttpGet("getallbookmarks")]
        public async Task<ActionResult<IEnumerable<Bookmark>>> GetAllBookmarksForUser()
        {
            if (!IsTokenValid() && !_signInManager.IsSignedIn(User))
            {
                return Unauthorized("Invalid access token");
            }

            string userId = _signInManager.UserManager.GetUserId(User);

            return await _bookmarkService.GetAllBookmarksForTheUser(userId);
        }

        // GET: api/bookmarks/getbookmark/5
        [HttpGet("getbookmark/{id}")]
        public async Task<ActionResult<Bookmark>> GetBookmark(int id)
        {
            if (!IsTokenValid() && !_signInManager.IsSignedIn(User))
            {
                return Unauthorized("Invalid access token");
            }

            var bookmark = await _bookmarkService.GetBookmark(id);

            if (bookmark == null)
            {
                return NotFound();
            }

            return bookmark;
        }

        // PUT: api/bookmarks/updatebookmark/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("updatebookmark/{id}")]
        public async Task<IActionResult> PutBookmark(int id, Bookmark bookmark)
        {
            if (!IsTokenValid() && !_signInManager.IsSignedIn(User))
            {
                return Unauthorized("Invalid access token");
            }

            if (id != bookmark.ID)
            {
                return BadRequest();
            }

            try
            {
                //var existingBookmark = await _bookmarkService.GetBookmark(bookmark.ID);
                //bookmark.CreateDate = existingBookmark.CreateDate;
                bookmark.LastUpdatedDate = DateTime.UtcNow;

                await _bookmarkService.UpdateBookmark(bookmark);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DBConcurrencyException("Concurrency issue");
            }

            return NoContent();
        }

        // POST: api/bookmarks/createbookmark
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("createbookmark")]
        public async Task<ActionResult<Bookmark>> PostBookmark(Bookmark bookmark)
        {
            if (!IsTokenValid() && !_signInManager.IsSignedIn(User))
            {
                return Unauthorized("Invalid access token");
            }

            bookmark.CreateDate = DateTime.UtcNow;
            bookmark.LastUpdatedDate = DateTime.UtcNow;

            await _bookmarkService.CreateBookmark(bookmark);

            return CreatedAtAction("GetBookmark", new { id = bookmark.ID }, bookmark);
        }

        // DELETE: api/bookmarks/deletebookmark/5
        [HttpDelete("deletebookmark/{id}")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            if (!IsTokenValid() && !_signInManager.IsSignedIn(User))
            {
                return Unauthorized("Invalid access token");
            }

            var bookmark = await _bookmarkService.GetBookmark(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            await _bookmarkService.DeleteBookmark(bookmark);

            return NoContent();
        }

        [NonAction]
        private bool IsTokenValid()
        {
            string accessToken = HttpContext.Request.Headers != null ? HttpContext.Request.Headers["Authorization"] : string.Empty;
            if (string.IsNullOrWhiteSpace(accessToken) || !accessToken.StartsWith("Bearer "))
            {
                return false;
            }

            string userEmail = HttpContext.Request.Headers != null ? HttpContext.Request.Headers["clientid"] : string.Empty;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return false;
            }

            IdentityUser user = _userManager.FindByEmailAsync(userEmail).Result;

            if(user == null)
            {
                return false;
            }

            //removing the Bearer token value.
            accessToken = accessToken.Replace("Bearer ","");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                _tokenValidationParameters.ValidateIssuer = true;
                _tokenValidationParameters.ValidateAudience = true;
                _tokenValidationParameters.ValidateLifetime = true;
                _tokenValidationParameters.ValidateIssuerSigningKey = true;
                _tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(user.Id));
                _tokenValidationParameters.ValidAudience = "test-audience";
                _tokenValidationParameters.ValidIssuer = "test-issuer";

                var token = tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var securityToken);

                var jwtToken = securityToken as JwtSecurityToken;

                if (jwtToken != null && jwtToken.ValidTo > DateTime.UtcNow)
                {
                    return true;
                }

                var userId = token.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch (Exception ex)
            {
                //catch, log exception
                Console.WriteLine(ex.ToString());
            }

            return false;
        }
    }
}
