using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.CategoryDtos;
using TicketingSys.Dtos.DepartmentDtos;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Enums;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Service;
using TicketingSys.Settings;

namespace TicketingSys.Controllers
{
    // NOTE:
    // routes that can be used by multiple user roles are here 
    [Route("shared")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly ISharedService _sharedService;
        private readonly IUserUtils _userUtils;
        private readonly IAttachmentService _attachmentService;
        private readonly ApplicationDbContext _context;

        public SharedController(ISharedService sharedService, IUserUtils userUtils,
               IAttachmentService attachmentService, ApplicationDbContext context)
        {
            _sharedService = sharedService;
            _userUtils = userUtils;
            _attachmentService = attachmentService;
            _context = context;
        }


        [Authorize]
        [HttpGet("myprofile")]
        public async Task<ActionResult<ViewUserDto>> myProfile()
        {
            var userId = _userUtils.getUserIdOr401();

            var user = await _sharedService.getUserById(userId);

            if (user is null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return user;
        }


        [Authorize(Policy = "AdminOrDepartmentUser")]
        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<ViewUserDto>> getProfileById(string userId)
        {
            var user = await _sharedService.getUserById(userId);

            if (user is null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return user;
        }

        // returns all tickets from the users department
        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpGet("alltickets")] 
        public async Task<ActionResult<List<ViewTicketDto>>> getAllTickets()
        {
            var userId = _userUtils.getUserIdOr401();

            var tickets = await _sharedService.getAllTicketsFromMyDepartment(userId);

            if (tickets == null || !tickets.Any())
                return NotFound("No tickets in your department");

            return Ok(tickets);
        }


        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpGet("querytickets")]
        public async Task<ActionResult<List<ViewTicketDto>>> GetAllTicketsWithQuery([FromQuery] SharedTicketQueryParamsDto query)
        {
            var currentUserId = _userUtils.getUserIdOr401(); 

            var results = await _sharedService.queryAlLTicketsFromMyDepartment(currentUserId, query);

            if (results == null || !results.Any())
                return NotFound("No tickets matching your criteria.");

            return Ok(results);
        }



        [Authorize(Policy = "AdminOrDepartmentUser")]
        [HttpGet("ticket/{ticketId}")]
        public async Task<ActionResult<ViewTicketDto?>> getTicketById(int ticketId)
        {
            var ticket = await _sharedService.getTicketById(ticketId);

            if (ticket == null)
                return NotFound("Ticket not found");

            var ticketDept = ticket.Department.Name.ToLowerInvariant();


            return Ok(ticket.modelToViewDto(includesResponses:true));
        }


        // returns all tickets submitted by a user, according to the roles of the currently logged in user
        [Authorize("AdminOrDepartmentUser")]
        [HttpGet("alltickets/{userId}")]
        public async Task<ActionResult<List<ViewTicketDto>>> getAllUsersTickets(string userId)
        {

            var tickets = await _sharedService.getAllTicketsFromUserByDepartment(userId);

            if (tickets == null || !tickets.Any())
                return NotFound("This user has no tickets");

            return Ok(tickets);
        }


        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpPatch("ticketstatus/{ticketId}")]
        public async Task<ActionResult<ViewTicketDto?>> changeTicketStatus(int ticketId, [FromBody] ChangeTicketStatusDto status)
        {
            var currentUserId = _userUtils.getUserIdOr401();

            var ticket = await _sharedService.changeTicketStatus(ticketId, status.Status, currentUserId);

            if (ticket == null) return NotFound();

            return Ok(ticket);
        }

        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpPatch("assigntome/{ticketId}")]
        public async Task<ActionResult<ViewTicketDto?>> assignTicketToMe(int ticketId)
        {
            var currentUserId = _userUtils.getUserIdOr401(); 

            var ticket = await _sharedService.assignTicketToUser(ticketId, currentUserId);

            if (ticket == null) return NotFound();

            return Ok(ticket);
        }

        // send response for a ticket
        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpPost("response")] 
        public async Task<IActionResult> AddResponse([FromBody] NewResponseDto dto)
        {
            var userId =_userUtils.getUserIdOr401();

            var referencedTicket = await _sharedService.getTicketById(dto.TicketId);

            if (referencedTicket is null) return BadRequest("Invalid ticket id");

            var savedResponse = await _sharedService.AddResponse(dto, userId);

            if (dto.Attachments != null && dto.Attachments.Any())
            {
                var attachments = await _attachmentService.CreateAndSaveResponseAttachments(savedResponse.Id, dto.Attachments);
                savedResponse.Attachments = attachments;
            }

            return Ok(savedResponse.ToViewDto());
        }



        // get all responses i sent - HR and IT only
        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpGet("sentresponses")]
        public async Task<ActionResult<List<ViewResponseDto>>> getSentResponses()
        {
            var userId = _userUtils.getUserIdOr401();

            var results = await _sharedService.getResponsesSentByUser(userId);

            if (results is null || results.Count == 0)
                return NotFound("You have no sent  responses");

            return Ok(results);
        }


        // should return response the hr or it user sent but double check if it works properly
        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpGet("sentresponse/{responseId}")]
        public async Task<ActionResult<ViewResponseDto?>> getSentResponse(int responseId)
        {

            var userId = _userUtils.getUserIdOr401();

            var response = await _sharedService.getSentResponseByUserIdAndResponseId(userId, responseId);

            if (response is null) return NotFound("You did not send a response with this id");

            return Ok(response);
        }


        [Authorize]
        [HttpGet("categories")]
        public async Task<ActionResult<List<ViewTicketCategoryDto>>> getAllCategories()
        {
            var categories = await _context.TicketCategories
           .Include(c => c.Department)
           .ToListAsync();

            var result = categories.Select(c => c.modelToViewDto());
            return Ok(result);
        }


        [Authorize]
        [HttpGet("categories/{categoryId}")]
        public async Task<ActionResult<ViewDepartmentDto?>> getCategoryById(int categoryId)
        {
            var result = await _sharedService.getCategoryById(categoryId);

            if (result == null) return NotFound();

            return Ok(result);
        }


        [Authorize]
        [HttpGet("{deptId}/categories")]
        public async Task<ActionResult<List<ViewTicketCategoryDto>>> getAllCategoriesById(int deptId)
        {
            var categories = await _context.TicketCategories.Where(c=> c.DepartmentId==deptId)
           .Include(c => c.Department)
           .ToListAsync();

            var result = categories.Select(c => c.modelToViewDto());
            return Ok(result);
        }


        [Authorize]
        [HttpGet("departments")]
        public async Task<ActionResult<List<ViewDepartmentDto>>> GetAllDepartments()
        {
            var results = await _sharedService.getAllDepartments();

            return Ok(results);
        }


        // returns departments user has access to by UserDepartmentAccess model
        [Authorize(Policy = "DepartmentUserOnly")]
        [HttpGet("mydepartments")]
        public async Task<ActionResult<List<ViewDepartmentDto>>> GetMyAssignedDepartments()
        {
            var userId = _userUtils.getUserIdOr401();

            var results = await _sharedService.getMyAssignedDepartments(userId);

            return Ok(results);
        }


        [Authorize]
        [HttpGet("departments/{departmentId}")]
        public async Task<ActionResult<ViewDepartmentDto?>> getDepartmentById(int departmentId)
        {
            var result = await _sharedService.getDepartmentById(departmentId);

            if (result == null) return NotFound();

            return Ok(result);
        }

    }
}
