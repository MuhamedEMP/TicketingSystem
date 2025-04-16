using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TicketingSys.Redis
{
    public class UserAccessCacheService: IUserAccessCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public UserAccessCacheService(IDistributedCache cache, ILogger<UserAccessCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<UserAccessCache?> GetUserAccessAsync(string userId)
        {
            var data = await _cache.GetStringAsync($"user-access:{userId}");
            if (string.IsNullOrEmpty(data))
            {
                _logger.LogInformation("USER-ACCESS:USERID IS NULL OR EMPTY");
                return null;
            }

            var dataSerialized = JsonSerializer.Deserialize<UserAccessCache>(data);

            _logger.LogInformation($"DATA IN GetUserAccessAsync {dataSerialized}");

            return dataSerialized;
        }

        public async Task SetUserAccessAsync(string userId, bool isAdmin, bool hasDepartmentAccess)
        {
            var data = new UserAccessCache
            {
                IsAdmin = isAdmin,
                HasDepartmentAccess = hasDepartmentAccess
            };

            await _cache.SetStringAsync($"user-access:{userId}",
                JsonSerializer.Serialize(data),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
                });
        }

        public async Task InvalidateUserAccessAsync(string userId)
        {
            await _cache.RemoveAsync($"user-access:{userId}");
        }


    }
}
