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


        public async Task<ViewUserDto> addUserAsync(string userId, string email, List<string> roles, string firstName,
            string fullName, string lastName)
        {
            // only admin role should exist in jwt but check just in case someone messes up azure AD
            if (!roles.Any(r =>
            r.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
            r.Equals("hr", StringComparison.OrdinalIgnoreCase) ||
            r.Equals("it", StringComparison.OrdinalIgnoreCase)))
                    {
                roles.Add("user"); // default user role
            }

            var user = new User
            {
                userId = userId,
                email = email,
                roles = roles,
                firstName = firstName,
                lastName = lastName,
                fullName = fullName
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
