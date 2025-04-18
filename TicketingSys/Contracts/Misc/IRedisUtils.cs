using Microsoft.EntityFrameworkCore;
using TicketingSys.Redis;

namespace TicketingSys.Contracts.Misc
{
    public interface IRedisUtils
    {
        Task<UserAccessCache?> GetOrFetchAccess(string userId);

    }
}
