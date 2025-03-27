using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TicketingSys.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController: ControllerBase
    {
        // Public Endpoint (No Authentication Required)
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "This is a public endpoint. No authentication required." });
        }

        [Authorize]
        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("manageronly")]
        public IActionResult ProtectedEndpoint()
        {
            var roles = User.FindAll("roles").Concat(User.FindAll("role"))
            .Select(r => r.Value)
            .Distinct();

            var roleList = string.Join(", ", roles);

            return Ok(new { message = $"This is a MANAGER ONLY ENDPOINT. Your roles are: {roleList}" });
        }

        [Authorize(Policy ="TestPolicy")]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 'sub' claim
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // 'email' claim
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(new { message = $"This is ADMIN ONLY ENDPOINT. your role from JWT is {role}!" });
        }

    }
}
