using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketingSys.Controllers
{
    // do not remove if debug - this controller will not appear in release
    #if DEBUG
    [ApiController]
    [Route("Test")]
    public class TestPolicyController : ControllerBase
    {   // 🔓 Any authenticated user
        [Authorize]
        [HttpGet("authenticated")]
        public IActionResult AuthenticatedOnly()
        {
            return Ok("✅ You are authenticated.");
        }

        // 🔐 Only Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok("✅ You are an Admin.");
        }

        // 👥 Only non-admin department users
        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpGet("department-user")]
        public IActionResult DepartmentUserOnly()
        {
            return Ok("✅ You are a department-level user (non-admin).");
        }

        // 🧑‍💻 Regular users with no department access
        [Authorize(Policy = "RegularUserOnly")]
        [HttpGet("regular-user")]
        public IActionResult RegularUserOnly()
        {
            return Ok("✅ You are a regular user (no dept, not admin).");
        }

        // 🔄 Either Admin or Department-level user
        [Authorize(Policy = "AdminOrDepartmentUser")]
        [HttpGet("admin-or-department")]
        public IActionResult AdminOrDepartmentUser()
        {
            return Ok("✅ You are an Admin or a department-level user.");
        }
    }
#endif
}

