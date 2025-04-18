using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.CategoryDtos;
using TicketingSys.Dtos.DepartmentDtos;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;
using TicketingSys.Util;

namespace TicketingSys.Controllers
{
    [Authorize(Policy = "RegularUserOnly")]
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttachmentService _attachmentService;
        private readonly IAdminService _adminService;
        private readonly IUserUtils _userUtils;
        private readonly ApplicationDbContext _context;
 
        public UserController(IUserService ticketService, IAttachmentService attachmentService,
                              IUserService userService, IUserUtils userUtils, ApplicationDbContext context)
        {
            _userService = ticketService;
            _attachmentService = attachmentService;
            _userService = userService;
            _userUtils = userUtils;
            _context = context;
        }


        [HttpPost("newticket/{departmentId}/{categoryId}")]
        public async Task<ActionResult<ViewTicketDto>> NewTicket([FromBody] NewTicketDto dto,
            int categoryId, int departmentId)
        {
            var userId = _userUtils.getUserIdOr401();

            var isValidCategory = await _userUtils.checkIfCategoryIsValid(categoryId: categoryId, departmentId: departmentId);

            if (isValidCategory == false)
                return BadRequest("Invalid category");

            var newTicket = dto.NewDtoToModel(userId, categoryId, departmentId);

            var savedTicket = await _userService.addNewTicket(newTicket);

            if (dto.Attachments != null && dto.Attachments.Any())
            {
                var attachments = await _attachmentService.CreateAndSaveTicketAttachments(savedTicket.Id, dto.Attachments);
                savedTicket.Attachments = attachments;
            }

            return Ok(savedTicket.modelToViewDto()); 
        }

       
        [HttpGet("tickets/{ticketId}")]
        public async Task<ActionResult<ViewTicketDto>> GetMyTicketById(int ticketId)
        {
            var userId = _userUtils.getUserIdOr401();

            var ticket = await _userService.getTicketByUserIdAndTicketId(userId, ticketId);

            if (ticket is null)
            {
                return NotFound($"Ticket with id {ticketId} not found");
            }

            return ticket.modelToViewDto();
        }


        [HttpGet("mytickets")]
        public async Task<ActionResult<List<ViewTicketDto>>> GetMyTickets([FromQuery] TicketQueryParamsDto queryDto)
        {
            var userId = _userUtils.getUserIdOr401();

            var results = await _userService.filterTickets(userId, queryDto);

            //if(!results.Any())
            //{
            //    return NotFound("No tickets matching your search.");
            //}

            return Ok(results);
        }


        // get responses to my tickets 
        [HttpGet("myresponses")]
        public async Task<ActionResult<List<ViewResponseDto>>> getMyResponses()
        {
            var userId = _userUtils.getUserIdOr401();

            var results = await _userService.getResponsesToUserTickets(userId);

            if (results is null || results.Count == 0)
                return NotFound("You have no responses");

            return Ok(results);
        }


        [HttpGet("recievedresponse/{responseId}")]
        public async Task<ActionResult<ViewResponseDto?>> getRecievedResponse(int responseId)
        {
            var userId = _userUtils.getUserIdOr401();

            var response = await _userService.GetResponseToMyTicketById(userId, responseId);

            if (response is null) return NotFound("You did not receive a response with this id");

            return Ok(response);

        }

        


    }
}
