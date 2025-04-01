using Microsoft.EntityFrameworkCore;
using System.Data;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Enums;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Service
{
    public class SharedService :ISharedService
    {

        private readonly ApplicationDbContext _context;

        public SharedService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response?> AddResponse(NewResponseDto dto, string userId, List<string> currentUserRoles)
        {
            var response = new Response
            {
                TicketId = dto.TicketId,
                UserId = userId,
                Message = dto.Message,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };


            await _context.Responses.AddAsync(response);

            var referencedTicket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == dto.TicketId);
            var departmentName = referencedTicket.Department.Name.ToLower();

            var userHasAccess = currentUserRoles
                .Any(role => role.Equals(departmentName, StringComparison.OrdinalIgnoreCase));

            if (!userHasAccess)
                return null;

            if (referencedTicket.Status != dto.Status)
                referencedTicket.Status = dto.Status;

            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<List<Ticket>?> getAllTicketByUserId(string userId)
        {
            return await _context.Tickets
            .Where(t => t.SubmittedById == userId)
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.AssignedTo)
            .Include(t => t.SubmittedBy)
            .Include(t => t.Attachments)
            .ToListAsync();
        }

        public async Task<List<ViewTicketDto>?> getAllTicketsFromMyDepartment(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.userId == userId);

            var normalizedRoles = user.roles
            .Select(role => role.ToLowerInvariant())
            .ToList();

            var tickets = await _context.Tickets
           .Include(t => t.SubmittedBy)
           .Include(t => t.AssignedTo)
           .Include(t => t.Category)
           .Include(t => t.Department)
           .Include(t => t.Attachments)
           .Where(t => normalizedRoles.Contains(t.Department.Name.ToLower()))
           .ToListAsync();

            if (tickets == null || tickets.Count == 0) return null;

            return tickets.modelToViewDtoList();
        }


        public async Task<List<ViewTicketDto>?> queryAlLTicketsFromMyDepartment(string currentUserId, SharedTicketQueryParamsDto query)
        {
            if (string.IsNullOrEmpty(currentUserId))
                return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId.ToLower() == currentUserId.ToLower());
            if (user == null || user.roles == null || user.roles.Count == 0)
                return null;

            var normalizedRoles = user.roles
                .Select(role => role.ToLowerInvariant())
                .ToList();

            var queryable = _context.Tickets
                .Include(t => t.SubmittedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Department)
                .Include(t => t.Attachments)
                .AsQueryable();

            // ✅ Filter by departments the current user is allowed to see
            queryable = queryable.Where(t => normalizedRoles.Contains(t.Department.Name.ToLower()));

            // ✅ Apply optional query filters
            if (query.Status.HasValue)
                queryable = queryable.Where(t => t.Status == query.Status.Value);

            if (query.Urgency.HasValue)
                queryable = queryable.Where(t => t.Urgency == query.Urgency.Value);

            if (!string.IsNullOrEmpty(query.AssignedToId))
                queryable = queryable.Where(t => t.AssignedToId == query.AssignedToId);

            if (!string.IsNullOrEmpty(query.UserId))
                queryable = queryable.Where(t => t.SubmittedById == query.UserId);

            if (query.CategoryId.HasValue)
                queryable = queryable.Where(t => t.CategoryId == query.CategoryId.Value);

            if (query.DepartmentId.HasValue)
                queryable = queryable.Where(t => t.DepartmentId == query.DepartmentId.Value);

            if (query.FromDate.HasValue)
                queryable = queryable.Where(t => t.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                queryable = queryable.Where(t => t.CreatedAt <= query.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(query.Search))
                queryable = queryable.Where(t =>
                    t.Title.ToLower().Contains(query.Search.ToLower()) ||
                    t.Description.ToLower().Contains(query.Search.ToLower())
                );

            var tickets = await queryable.ToListAsync();
            return tickets.Any() ? tickets.modelToViewDtoList() : null;
        }


        public async Task<List<ViewResponseDto>> getResponsesSentByUser(string userId)
        {
            var responsesSentByUser = await _context.Responses
            .Include(r => r.Ticket)
            .Include(r => r.User)
            .Include(r => r.Attachments)
            .Where(r => r.UserId == userId)
            .ToListAsync();

            return responsesSentByUser.Select(r => r.ToViewDto()).ToList();
        }

        public async Task<ViewResponseDto?> getSentResponseByUserIdAndResponseId(string userId, int responseId)
        {
            var response = await _context.Responses
            .Include(r => r.Ticket)
            .Include(r => r.User)
            .Include(r => r.Attachments)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Id == responseId);

            if (response == null)
                return null;

            return response.ToViewDto();
        }

        public async Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId)
        {
            // includes object references by FK
            return await _context.Tickets.Include(t => t.SubmittedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == ticketId && t.SubmittedById == userId);
        }

        public async Task<Ticket?> getTicketById(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.SubmittedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Department)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return null;

            return ticket;
        }

        public async Task<ViewUserDto?> getUserById(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == id);
            if (user == null) return null;
            return user.userModelToDto();
        }

        public async Task<List<ViewTicketDto>?> getAllTicketsFromUserByDepartment(string userId, List<string> currentUserRoles)
        {
            if (currentUserRoles.Contains("admin"))
            {
                var allTickets = await _context.Tickets
                    .Include(t => t.SubmittedBy)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.Category)
                    .Include(t => t.Department)
                    .Include(t => t.Attachments)
                    .Where(t => t.SubmittedById == userId)
                    .ToListAsync();

                return allTickets.modelToViewDtoList();
            }


            var tickets = await _context.Tickets
                .Include(t => t.SubmittedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Department)
                .Include(t => t.Attachments)
                .Where(t =>
                    t.SubmittedById == userId &&
                    currentUserRoles.Contains(t.Department.Name.ToLower()))
                .ToListAsync();


            if (tickets.Any()) return tickets.modelToViewDtoList();
            return null;
        }

        public async Task<ViewTicketDto?> changeTicketStatus(int ticketId, TicketStatusEnum status, List<string> currentUserRoles)
        {
            // include object references
            var ticket = await _context.Tickets
            .Include(t => t.Department)
            .Include(t => t.SubmittedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Category)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return null;

            var departmentName = ticket.Department.Name.ToLower();
            var userHasAccess = currentUserRoles
                .Any(role => role.Equals(departmentName, StringComparison.OrdinalIgnoreCase) || role.Equals("admin", StringComparison.OrdinalIgnoreCase));

            if (!userHasAccess) return null;

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ticket.modelToViewDto();
        }

        public async Task<ViewTicketDto?> assignTicketToUser(int ticketId, string userId, List<string> currentUserRoles)
        {
            // include object references
            var ticket = await _context.Tickets
            .Include(t => t.Department)
            .Include(t => t.SubmittedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Category)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return null;

            var departmentName = ticket.Department.Name.ToLower();
            var userHasAccess = currentUserRoles
                .Any(role => role.Equals(departmentName, StringComparison.OrdinalIgnoreCase) || role.Equals("admin", StringComparison.OrdinalIgnoreCase));

            if (!userHasAccess) return null;

            ticket.AssignedToId = userId;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ticket.modelToViewDto();
        }

    }
}
