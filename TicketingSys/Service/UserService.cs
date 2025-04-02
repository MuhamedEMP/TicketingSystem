
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Response> AddResponse(NewResponseDto dto, string userId)
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

            if (referencedTicket.Status != dto.Status)
                referencedTicket.Status = dto.Status;

            await _context.SaveChangesAsync();

            return response;
        }


        public async Task<Ticket> addNewTicket(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        // nullable so error can be returned in controller if user is null
        public async Task<User?> getUserById(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.userId == id);
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

        public async Task<Ticket?> getTikcketById(int ticketId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

            if (ticket is null) return null;

            return ticket;
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

        public async Task<List<ViewTicketDto>?> filterTickets(string userId, TicketQueryParamsDto filters)
        {
            var query = _context.Tickets
            .Include(t => t.SubmittedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.Attachments)
            .Where(t => t.SubmittedById == userId)
            .AsQueryable();


            if (!string.IsNullOrEmpty(userId))
                query = query.Where(t => t.SubmittedById == userId);

            if (filters.Status.HasValue)
                query = query.Where(t => t.Status == filters.Status.Value);

            if (filters.Urgency.HasValue)
                query = query.Where(t => t.Urgency == filters.Urgency.Value);

            if (!string.IsNullOrEmpty(filters.AssignedToId))
                query = query.Where(t => t.AssignedToId == filters.AssignedToId);

            if (filters.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == filters.CategoryId.Value);

            if (filters.DepartmentId.HasValue)
                query = query.Where(t => t.DepartmentId == filters.DepartmentId.Value);

            if (filters.FromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= filters.FromDate.Value);

            if (filters.ToDate.HasValue)
                query = query.Where(t => t.CreatedAt <= filters.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var search = filters.Search.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(search) ||
                    t.Description.ToLower().Contains(search));
            }

            var tickets = await query.ToListAsync();
            return tickets.Select(t => t.modelToViewDto()).ToList();
        }

        public async Task<List<ViewResponseDto>> getResponsesToUserTickets(string userId)
        {
            var responsesToUserTickets = await _context.Responses
                .Include(r => r.Ticket)
                .ThenInclude(t => t.SubmittedBy)
                .Include(r => r.User)
                .Include(r => r.Attachments)
                .Where(r => r.Ticket.SubmittedById == userId)
                .ToListAsync();

            return responsesToUserTickets.Select(r => r.ToViewDto()).ToList();
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


        public async Task<ViewResponseDto?> GetResponseToMyTicketById(string userId, int responseId)
        {
            var response = await _context.Responses
                .Include(r => r.Ticket)
                .Include(r => r.User)
                .Include(r => r.Attachments)
                .FirstOrDefaultAsync(r => r.Ticket.SubmittedById == userId && r.Id == responseId);

            if (response == null)
                return null;

            return response.ToViewDto();
        }
    }
}