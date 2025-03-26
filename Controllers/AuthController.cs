using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _db;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
            
        }

        // testing endpoint
        [Authorize]
        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }


        // not finalized yet but it works
        [HttpGet("register")]
        public async Task<IActionResult> registerUser()
        {
            // sub is the user id
            var sub = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var email = User.FindFirst("unique_name")?.Value
                      ?? User.FindFirst(ClaimTypes.Email)?.Value;

            // list of roles the user has
            var roles = User.FindAll("roles").Concat(User.FindAll("role"))
                .Select(r => r.Value)
                .Distinct()
                .ToList();


            var firstName = User.FindFirst("given_name")?.Value;
            var lastName = User.FindFirst("family_name")?.Value;
            var fullName = User.FindFirst("name")?.Value;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.userId == sub);
            if (user == null)
            {
                user = new User
                {
                    userId = sub,
                    email = email,
                    firstName = firstName,
                    lastName = lastName,
                    fullName = fullName,
                    roles = roles
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return Ok("USER CREATED");
            }

            return Ok($"User EXISTS WITH details :sub-{sub} email-{email} fn-{firstName} ln-{lastName} full-{fullName}, roles {roles}");
        }


    }
}
