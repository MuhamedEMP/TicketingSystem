
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketingSys.Contracts.RepositoryInterfaces;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;


namespace TicketingSys.Controllers
{

    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService ticketService, IAttachmentRepository attachmentRepository,
            UserManager<User> userManager)
        {
            _userService = ticketService;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
        }

        [HttpGet("register")]
        public async Task<IActionResult> Register()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated.");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var fullName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            {
                return BadRequest($"User claims not found. email {email} - user id {userId}");
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }

            var user = new User
            {
                Id = userId,
                UserName =fullName,
                Email= email
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(user);
        }

        

        [HttpPost("newticket")]
        public async Task<IActionResult> NewTicket([FromBody] NewTicketDto dto)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var newTicket = dto.NewDtoToModel(userId);

            Console.Write($"user id is : {userId}");

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
    }
}
