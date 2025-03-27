using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Util;

namespace TicketingSys.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttachmentService _attachmentService;
        private readonly IAdminService _adminService;
        private readonly IUserUtils _userUtils;
        public UserController(IUserService ticketService, IAttachmentService attachmentService,
                              IUserService userService, IUserUtils userUtils)
        {
            _userService = ticketService;
            _attachmentService = attachmentService;
            _userService = userService;
            _userUtils = userUtils;
        }


        [Authorize]
        [HttpPost("newticket")]
        public async Task<IActionResult> NewTicket([FromBody] NewTicketDto dto)
        {
            var userId = _userUtils.getUserId();

            var newTicket = dto.NewDtoToModel(userId);

            await _userService.addNewTicket(newTicket);

            foreach (var attachmentDto in dto.Attachments)
            {
                var attachment = new TicketAttachment
                {
                    TicketId = newTicket.Id,  // Set the TicketId of the saved ticket
                    Path = attachmentDto.Path,
                    FileName = attachmentDto.Filename,
                    ContentType = attachmentDto.ContentType
                };

                // Add each attachment to the ticket's attachment list (optional)
                newTicket.Attachments.Add(attachment);
            }

            await _attachmentService.SaveAttachments(newTicket.Attachments);

            return Ok(newTicket);
        }

        [Authorize]
        [HttpGet("myprofile")]
        public async Task<ActionResult<ViewUserDto>>  myProfile()
        {
            var userId = _userUtils.getUserId();

            var user = await _userService.getUserById(userId);

            if (user is null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return user.userModelToDto();
        }

        [Authorize]
        [HttpGet("tickets/{ticketId}")]
        public async Task<ActionResult<ViewTicketDto>> GetMyTicketById(int ticketId)
        {
            var userId = _userUtils.getUserId();

            var ticket = await _userService.getTicketByUserIdAndTicketId(userId, ticketId);

            if (ticket is null)
            {
                return NotFound($"Ticket with id {ticketId} not found");
            }

            return ticket.modelToViewDto();
        }


        [Authorize]
        [HttpGet("mytickets")]
        public async Task<ActionResult<List<ViewTicketDto>>> GetAllMyTickets()
        {
            var userId = _userUtils.getUserId();

            var tickets = await _userService.getAllTicketByUserId(userId);

            if (!tickets.Any())
            {
                return NotFound("You have no tickets");
            }

            return tickets.modelToViewDtoList();
        }


        [Authorize]
        [HttpGet("filter")]
        public async Task<ActionResult<List<ViewTicketDto>>> FilterMyTickets([FromQuery] TicketQueryParamsDto queryDto)
        {
            var userId = _userUtils.getUserId();

            var results = await _userService.filterTickets(userId, queryDto);

            if(!results.Any())
            {
                return NotFound("No tickets found");
            }

            return Ok(results);
        }
    }
}
