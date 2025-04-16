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
        private readonly ILogger _logger;

        public RedisUtils(IUserAccessCacheService cache, ApplicationDbContext context, ILogger<RedisUtils> logger)
        {
            _cacheService = cache;
            _context = context;
            _logger = logger;
        }


        public async Task<UserAccessCache?> GetOrFetchAccess(string userId)
        {
            var access = await _cacheService.GetUserAccessAsync(userId);
            if (access != null)
            {
                _logger.LogWarning("ACCESS FROM REDIS IS NOT NULL: isAdmin = {IsAdmin}, hasDept = {HasDept}",
                    access.IsAdmin, access.HasDepartmentAccess);

                return access;
            }
                

            _logger.LogWarning("ACCESS IS NULL");
            var user = await _context.Users.Include(u => u.DepartmentAccesses)
                .FirstOrDefaultAsync(u => u.userId == userId);
            if (user is null) return null;

            access = new UserAccessCache
            {
                IsAdmin = user.IsAdmin,
                HasDepartmentAccess = user.DepartmentAccesses.Any()
            };

            _logger.LogWarning($"FROM DATABASE id: {userId}, isadmin: {user.IsAdmin}, access: {user.DepartmentAccesses.Any()}");

            await _cacheService.SetUserAccessAsync(userId, access.IsAdmin, access.HasDepartmentAccess);
            return access;
        }

    }
}
