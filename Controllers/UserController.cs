using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketingSys.Contracts.RepositoryInterfaces;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly IAttachmentRepository _attachmentRepository;
        public UserController(IUserService ticketService, IAttachmentRepository attachmentRepository, ApplicationDbContext db)
        {
            _userService = ticketService;
            _attachmentRepository = attachmentRepository;;
            _db = db;
        }

        // not finalized yet but it works
        [HttpGet("register")]
        public async Task<IActionResult> registerUser()
        {
            // sub is the user id
            var sub = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var email = User.FindFirst("unique_name")?.Value
                      ?? User.FindFirst(ClaimTypes.Email)?.Value;

            // list of roles the user has
            var roles = User.FindAll("roles").Concat(User.FindAll("role"))
                .Select(r => r.Value)
                .Distinct()
                .ToList();


            var firstName = User.FindFirst("given_name")?.Value;
            var lastName = User.FindFirst("family_name")?.Value;
            var fullName = User.FindFirst("name")?.Value;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.userId == sub);
            if (user == null)
            {
                user = new User { userId = sub, email = email,
                    firstName = firstName, lastName = lastName, fullName = fullName, roles = roles};
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return Ok("USER CREATED");
            }

            return Ok($"User EXISTS WITH details :sub-{sub} email-{email} fn-{firstName} ln-{lastName} full-{fullName}, roles {roles}");
            }


        // testing endpoint
        [Authorize]
        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }

        [Authorize]
        [HttpPost("newticket")]
        public async Task<IActionResult> NewTicket([FromBody] NewTicketDto dto)
        {
            // sub is the user id
            var userId = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
