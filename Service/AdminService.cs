using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Settings;

namespace TicketingSys.Service
{
    public class AdminService : IAdminService
    {
        ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<ViewUserDto>?> getAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => u.userModelToDto()).ToList();
        }
    }
}
