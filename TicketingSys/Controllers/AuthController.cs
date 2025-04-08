using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketingSys.Contracts.Misc;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly IUserUtils _userUtils;

        public AuthController(ApplicationDbContext db, IUserUtils userUtils)
        {
            _db = db;
            _userUtils = userUtils;
            
        }


        // not finalized yet but it works
        [HttpGet("register")]
        public async Task<IActionResult> registerUser()
        {
            // sub is the user id
            var sub = _userUtils.getUserIdOr401();
            var email = User.FindFirst("unique_name")?.Value
                      ?? User.FindFirst(ClaimTypes.Email)?.Value;

            // list of roles the user has
            var roles = User.FindAll("roles").Concat(User.FindAll("role"))
                .Select(r => r.Value)
                .Distinct()
                .ToList();

            if (roles.Count == 0) roles.Add("user");


            var firstName = User.FindFirst("given_name")?.Value;
            var lastName = User.FindFirst("family_name")?.Value;
            var fullName = User.FindFirst("name")?.Value;


            if (string.IsNullOrEmpty(sub))
            {
                return BadRequest("❌ Token is missing 'sub' claim or user is not authenticated.");
            }


            //return Ok((sub, email, roles, fullName).ToString());

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
