using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.UserDtos;

namespace TicketingSys.Controllers
{
    [Route("admin")]
    [ApiController]
    [Authorize(Policy ="AdminFromDb")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        [HttpGet("users")]
        public async Task<ActionResult<List<ViewUserDto>>> viewAllUsers()
        {
            var users = await _adminService.getAllUsers();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found");
            }

            return users;

        }


        //[HttpGet("myprofile")]
        //public async 
    }
}
