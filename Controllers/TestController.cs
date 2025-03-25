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


        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(new { message = $"This is a MANAGER ONLY ENDPOINT. your role is {role}!" });
        }

        [Authorize(Policy ="AdminOnly")]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 'sub' claim
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // 'email' claim
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(new { message = $"This is ADMIN ONLY ENDPOINT. your role is {role}!" });
        }
    }
}
