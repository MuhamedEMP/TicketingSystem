using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;

namespace TicketingSys.Controllers
{
    [Route("admin")]
    [ApiController]
    //[Authorize(Policy ="AdminFromDb")]
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


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ViewUserDto>> viewUser(string userId)
        {
            var user = await _adminService.getUserById(userId);
            if (user is null)
            {
                return NotFound($"User with id {userId} not found");
            }

            return Ok(user);
        }


        [HttpPatch("changerole")]
        public async Task<ActionResult<ViewUserDto>> changeUserRole([FromBody] ChangeRoleDto dto)
        {
            var user = await _adminService.changeRole(dto);

            if (user is null)
            {
                return NotFound($"User with id {dto.userId} not found");
            }

            return Ok(user);
        }

        [HttpPost("adddepartment/{deptName}")]
        public async Task<ActionResult> addDepartment(string deptName)
        {
            string lowercase = deptName.ToLower();

            await _adminService.addDepartment(lowercase);

            return Ok("Created department with name "+lowercase);
        }


        [HttpPost("addcategory")]
        public async Task<ActionResult<NewTicketCategoryDto>> addCategory([FromBody] NewTicketCategoryDto dto)
        {
            await _adminService.addCategory(dto);
            return Ok($"Added category with name:{dto.Name} and description {dto.Description}");
        }

    }
}
