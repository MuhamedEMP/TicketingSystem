using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Service
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<ViewUserDto> addUserAsync(string userId, string email, string firstName,
            string fullName, string lastName)
        {
        

            var user = new User
            {
                userId = userId,
                email = email,
                firstName = firstName,
                lastName = lastName,
                fullName = fullName,
                IsAdmin = false // False by default
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.userModelToDto();
        }


        public async Task<bool> checkIfUserExists(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            return true;
        }
    }
}
