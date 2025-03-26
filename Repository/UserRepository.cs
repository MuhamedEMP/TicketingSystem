using Microsoft.AspNetCore.Mvc;
using TicketingSys.Contracts.RepositoryInterfaces;
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
    }
}
