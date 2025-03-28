using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.ResponseDtos;
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

            var savedTicket = await _userService.addNewTicket(newTicket);

            if (dto.Attachments != null && dto.Attachments.Any())
            {
                var attachments = await _attachmentService.CreateAndSaveTicketAttachments(savedTicket.Id, dto.Attachments);
                savedTicket.Attachments = attachments;
            }

            return Ok(savedTicket); // Or map to a DTO before returning
        }

        // only for IT and HR
        [Authorize]
        [HttpPost("response")]
        public async Task<IActionResult> AddResponse([FromBody] NewResponseDto dto)
        {
            var userId = _userUtils.getUserId();

            var referencedTicket = await _userService.getTikcketById(dto.TicketId);
            
            if (referencedTicket is null) return BadRequest("Invalid ticket id");

            var savedResponse = await _userService.AddResponse(dto, userId);

            if (dto.Attachments != null && dto.Attachments.Any())
            {
                var attachments = await _attachmentService.CreateAndSaveResponseAttachments(savedResponse.Id, dto.Attachments);
                savedResponse.Attachments = attachments;
            }

            return Ok(savedResponse); 
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

        // get responses to my tickets not the responses i sent 
        [Authorize]
        [HttpGet("myresponses")]
        public async Task<ActionResult<List<ViewResponseDto>>> getMyResponses()
        {
            var userId = _userUtils.getUserId();

            var results = await _userService.getResponsesToUserTickets(userId);

            if (results is null || results.Count == 0)
                return NotFound("You have no responses");

            return Ok(results);
        }

        // get all responses i sent - HR and IT only
        [Authorize]
        [HttpGet("sentresponses")]
        public async Task<ActionResult<List<ViewResponseDto>>> getSentResponses()
        {
            var userId = _userUtils.getUserId();

            var results = await _userService.getResponsesSentByUser(userId);

            if (results is null || results.Count == 0)
                return NotFound("You have no sent  responses");

            return Ok(results);
        }


        // should return response the hr or it user sent but double check if it works properly
        [Authorize]
        [HttpGet("sentresponse/{responseId}")]
        public async Task<ActionResult<ViewResponseDto?>> getSentResponse(int responseId)
        {

            var userId = _userUtils.getUserId();

            var response = await _userService.getSentResponseByUserIdAndResponseId(userId, responseId);

            if (response is null) return NotFound("You did not send a response with this id");

            return Ok(response);
        }

        [Authorize]
        [HttpGet("recievedresponse/{responseId}")]
        public async Task<ActionResult<ViewResponseDto?>> getRecievedResponse(int responseId)
        {
            var userId = _userUtils.getUserId();

            var response = await _userService.GetResponseToMyTicketById(userId, responseId);

            if (response is null) return NotFound("You did not receive a response with this id");

            return Ok(response);

        }
    }
}
