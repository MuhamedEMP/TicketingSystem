using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Claims;
using TicketingSys.Contracts.RepositoryInterfaces;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        public UserController(IUserService ticketService, IAttachmentRepository attachmentRepository,
                              IUserRepository userRepository)
        {
            _userService = ticketService;
            _attachmentRepository = attachmentRepository;
            _userRepository = userRepository;
        }


        [Authorize]
        [HttpPost("newticket")]
        public async Task<IActionResult> NewTicket([FromBody] NewTicketDto dto)
        {
            // sub is the user id
            var userId = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var newTicket = dto.NewDtoToModel(userId);

            await _userService.newTicket(newTicket);

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

            await _attachmentRepository.SaveAttachments(newTicket.Attachments);

            return Ok(newTicket);
        }

        [Authorize]
        [HttpGet("myprofile")]
        public async Task<ActionResult<ViewUserDto>>  myProfile()
        {
            // sub is the user id
            var userId = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userRepository.getUserById(userId);

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
            // sub is the user id
            var userId = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var ticket = await _userRepository.getTicketByUserIdAndTicketId(userId, ticketId);

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
            // sub is the user id
            var userId = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var tickets = await _userRepository.getAllTicketByUserId(userId);

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
            // sub is the user id
            var userId = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var results = await _userService.filterTickets(userId, queryDto);

            if(!results.Any())
            {
                return NotFound("No tickets found");
            }

            return Ok(results);
        }
    }
}
