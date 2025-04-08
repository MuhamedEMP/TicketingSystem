
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;
using TicketingSys.Utils;

namespace TicketingSys.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
      

        public async Task<Ticket> addNewTicket(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
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


        public async Task<List<Ticket>?> getAllTicketByUserId(string userId)
        {
            var ticket = await _context.Tickets
            .Where(t => t.SubmittedById == userId)
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.AssignedTo)
            .Include(t => t.SubmittedBy)
            .Include(t => t.Attachments)
            .ToListAsync();

            return ticket.SortByStatusAndUrgency();
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

            if (filters.Status.HasValue)
                query = query.Where(t => t.Status == filters.Status.Value);

            if (filters.Urgency.HasValue)
                query = query.Where(t => t.Urgency == filters.Urgency.Value);

            if (!string.IsNullOrWhiteSpace(filters.AssignedToName))
            {
                string assignedTo = filters.AssignedToName.ToLower();
                query = query.Where(t => t.AssignedTo != null && t.AssignedTo.fullName.ToLower().Contains(assignedTo));
            }

            if (!string.IsNullOrWhiteSpace(filters.CategoryName))
            {
                string category = filters.CategoryName.ToLower();
                query = query.Where(t => t.Category != null && t.Category.Name.ToLower().Contains(category));
            }

            if (!string.IsNullOrWhiteSpace(filters.DepartmentName))
            {
                string department = filters.DepartmentName.ToLower();
                query = query.Where(t => t.Department != null && t.Department.Name.ToLower().Contains(department));
            }

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
            var sorted = tickets.SortByStatusAndUrgency();

            return sorted.Select(t => t.modelToViewDto()).ToList();
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