using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Models;
using TicketingSys.Service;
using TicketingSys.Settings;

namespace TicketingSys.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly IUserUtils _userUtils;
        private readonly IAuthService _authService;

        public AuthController(ApplicationDbContext db, IUserUtils userUtils, IAuthService authService)
        {
            _db = db;
            _userUtils = userUtils;
            _authService = authService;
        }


        [HttpGet("register")]
        public async Task<IActionResult> registerUser()
        {
            // sub is the user id
            var sub = _userUtils.getUserIdOr401();

            var email = User.FindFirst("unique_name")?.Value
                      ?? User.FindFirst(ClaimTypes.Email)?.Value;

            // each user will have at least empty list of roles in jwt
            var roles = User.FindAll("roles").Concat(User.FindAll("role"))
                .Select(r => r.Value)
                .Distinct()
                .ToList();       


            var firstName = User.FindFirst("given_name")?.Value;
            var lastName = User.FindFirst("family_name")?.Value;
            var fullName = User.FindFirst("name")?.Value;


            if (string.IsNullOrWhiteSpace(sub) ||
                string.IsNullOrWhiteSpace(email) ||
                roles == null ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(fullName))
            {
                return Unauthorized();
            }

            var exists = await _authService.checkIfUserExists(sub);
            if (exists is true) return Ok("User exists");

            var result = await _authService.addUserAsync(sub, email, firstName, fullName, lastName);
            return Ok("User created");
        }

    }
}
