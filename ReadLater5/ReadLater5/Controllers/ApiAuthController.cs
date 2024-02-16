using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReadLater5.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace ReadLater5.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class ApiAuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        public ApiAuthController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        //create get request for validating and generating token
        [HttpPost("oauth")]
        public async Task<IActionResult> GetAuthToken([FromBody] LoginModel loginModel)
        {
            if(loginModel == null || string.IsNullOrWhiteSpace(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Password)) 
            {
                return BadRequest();
            }

            //validate logins
            var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);

            if(result.Succeeded)
            {
                var userId = _signInManager.UserManager.Users.FirstOrDefault(u => u.UserName == loginModel.Username).Id;
                //Generate JWT token
                string token = GenerateJwtToken(userId);
                return Ok(new { Access_Token = token, ExpiresIn = 15, Type = "Bearer" });
            }

            return Unauthorized();
        }

        [NonAction]
        private string GenerateJwtToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(userId);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = "test-audience",
                Issuer = "test-issuer"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
