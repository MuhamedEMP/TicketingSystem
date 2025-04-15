using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.Misc;
using TicketingSys.Redis;
using TicketingSys.Settings;

namespace TicketingSys.Utils
{
    public class RedisUtils: IRedisUtils
    {
        private readonly IUserAccessCacheService _cacheService;
        private readonly ApplicationDbContext _context;

        public RedisUtils(IUserAccessCacheService cache, ApplicationDbContext context)
        {
            _cacheService = cache;
            _context = context;
        }


        public async Task<UserAccessCache?> GetOrFetchAccess(string userId)
        {
            var access = await _cacheService.GetUserAccessAsync(userId);
            if (access != null) return access;

            var user = await _context.Users.Include(u => u.DepartmentAccesses)
                .FirstOrDefaultAsync(u => u.userId == userId);
            if (user is null) return null;

            access = new UserAccessCache
            {
                IsAdmin = user.IsAdmin,
                HasDepartmentAccess = user.DepartmentAccesses.Any()
            };

            await _cacheService.SetUserAccessAsync(userId, access.IsAdmin, access.HasDepartmentAccess);
            return access;
        }

    }
}
