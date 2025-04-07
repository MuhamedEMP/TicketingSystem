using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketingSys.Controllers
{
    // do not remove if debug - this controller will not appear in release
    #if DEBUG
    [ApiController]
    [Route("Test")]
    public class TestPolicyController : ControllerBase
    {

        [Authorize(Policy = "AdminFromDb")]
        [HttpGet("admin")]
        public IActionResult Admin() => Ok("Admin access");

        [Authorize(Policy = "HrFromDb")]
        [HttpGet("hr")]
        public IActionResult Hr() => Ok("HR access");

        [Authorize(Policy = "ItFromDb")]
        [HttpGet("it")]
        public IActionResult It() => Ok("IT access");

        [Authorize(Policy = "UserFromDb")]
        [HttpGet("user")]
        public IActionResult User() => Ok("User access");

        [Authorize(Policy = "AdminHrItFromDb")]
        [HttpGet("adminhrit")]
        public IActionResult AdminHrIt() => Ok("Admin/HR/IT access");

        [Authorize(Policy = "HrOrIt")]
        [HttpGet("hrorit")]
        public IActionResult HrOrIt() => Ok("HR or IT access");

        [Authorize(Policy = "HrOrAdmin")]
        [HttpGet("hroradmin")]
        public IActionResult HrOrAdmin() => Ok("HR or Admin access");

        [Authorize(Policy = "ItOrAdmin")]
        [HttpGet("itoradmin")]
        public IActionResult ItOrAdmin() => Ok("IT or Admin access");

        [Authorize(Policy = "HrOrIT")]
        [HttpGet("hrorit_duplicate")]
        public IActionResult HrOrItDuplicate() => Ok("HR or IT (duplicate) access");

        [Authorize(Policy = "AllRoles")]
        [HttpGet("allroles")]
        public IActionResult AllRoles() => Ok("All roles access");
    }
#endif
}

