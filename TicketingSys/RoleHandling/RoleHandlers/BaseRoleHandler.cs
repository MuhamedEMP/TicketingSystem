using Microsoft.AspNetCore.Authorization;
using TicketingSys.Contracts.Misc;
using TicketingSys.Settings;

namespace TicketingSys.RoleHandling.RoleHandlers
{
    public abstract class BaseRoleHandler<TRequirement> : 
        AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IRedisUtils _redisUtils;
        protected readonly ILogger _logger;

        protected BaseRoleHandler(ApplicationDbContext context, IRedisUtils redisUtils, ILogger logger)
        {
            _context = context;
            _redisUtils = redisUtils;
            _logger = logger;
        }

    }
}
