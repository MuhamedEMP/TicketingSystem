using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketingSys.Contracts.Misc;
using TicketingSys.Models;
using TicketingSys.Settings;


namespace TicketingSys.Util
{

    public class UserUtils : IUserUtils
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        public UserUtils(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public string? getUserId()
        { // sub is the user id
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst("sub")?.Value
                ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<List<string>?> getUserRoles()
        {
            var userId = getUserId();
            if (userId == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u=> u.userId == userId);

            if (user == null) return null;

            var normalizedRoles = user.roles
                .Select(role => role.ToLowerInvariant())
                .ToList();

            return normalizedRoles;

        }
    }

}
