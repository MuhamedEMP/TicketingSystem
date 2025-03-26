using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.RepositoryInterfaces;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;
using TicketingSys.Settings;

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
    }
}