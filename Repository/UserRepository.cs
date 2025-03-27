using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.RepositoryInterfaces;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;
using TicketingSys.Settings;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TicketingSys.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> addNewTask(Ticket ticket)
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

        public async Task<List<Ticket>?> filterTickets(string userId, TicketQueryParamsDto filters)
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

            return await query.ToListAsync();
        }
    }
}