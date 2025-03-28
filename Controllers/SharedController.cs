using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Service;

namespace TicketingSys.Controllers
{   
    // routes that can be used by multiple user roles are here

    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly ISharedService _sharedService;
        private readonly IUserUtils _userUtils;

        public SharedController(ISharedService sharedService, IUserUtils userUtils)
        {
            _sharedService = sharedService;
            _userUtils = userUtils;
        }

        [Authorize(Policy ="AllRoles")]
        [HttpGet("myprofile")]
        public async Task<ActionResult<ViewUserDto>> myProfile()
        {
            var userId = _userUtils.getUserId();

            var user = await _sharedService.getUserById(userId);

            if (user is null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return user.userModelToDto();
        }


        [Authorize(Policy = "AdminHrItFromDb")]
        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<ViewUserDto>> myProfile(string userId)
        {
            var user = await _sharedService.getUserById(userId);

            if (user is null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return user.userModelToDto();
        }

        // returns all tickets from the users department
        [Authorize(Policy = "HrOrIt")]
        [HttpGet("alltickets")] 
        public async Task<ActionResult<List<ViewTicketDto>>> getAllTickets()
        {
            var userId = _userUtils.getUserId();

            var tickets = await _sharedService.getAllTicketsFromMyDepartment(userId);

            if (tickets == null || !tickets.Any())
                return NotFound("No tickets in your department");

            return Ok(tickets);
        }


        [Authorize(Policy = "HrOrIt")]
        [HttpGet("querytickets")]
        public async Task<ActionResult<List<ViewTicketDto>>> GetAllTicketsWithQuery([FromQuery] SharedTicketQueryParamsDto query)
        {
            var currentUserId = _userUtils.getUserId(); 

            var results = await _sharedService.queryAlLTicketsFromMyDepartment(currentUserId, query);

            if (results == null || !results.Any())
                return NotFound("No tickets matching your criteria.");

            return Ok(results);
        }



        [Authorize(Policy = "AdminHrItFromDb")]
        [HttpGet("ticket/{ticketId}")]
        public async Task<ActionResult<ViewTicketDto?>> getTicketById(int ticketId)
        {
            var userId = _userUtils.getUserId();

            var user = await _sharedService.getUserById(userId);

            var normalizedRoles = user.roles
                .Select(role => role.ToLowerInvariant())
                .ToList();

            var ticket = await _sharedService.getTicketById(ticketId);

            if (ticket == null)
                return NotFound("Ticket not found");

            var ticketDept = ticket.Department.Name.ToLowerInvariant();

            bool isAdmin = normalizedRoles.Contains("admin");
            bool isInSameDept = ticketDept != null && normalizedRoles.Contains(ticketDept);

            if (!isAdmin && !isInSameDept)
                return Forbid();

            return Ok(ticket.modelToViewDto());
        }



    }
}
