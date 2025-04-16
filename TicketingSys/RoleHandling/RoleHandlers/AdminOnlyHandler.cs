using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TicketingSys.RoleHandling.Policies;
using TicketingSys.Settings;
using TicketingSys.Redis;
using TicketingSys.Contracts.Misc;

namespace TicketingSys.RoleHandling.RoleHandlers
{
    // only admin with bool isAdmin = true user can access the routes
    public class AdminOnlyHandler: BaseRoleHandler<AdminOnlyRequirement>
    {
        public AdminOnlyHandler(ApplicationDbContext context, IRedisUtils redisUtils, ILogger<AdminOnlyHandler> logger)
        : base(context, redisUtils, logger) { }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AdminOnlyRequirement requirement)
        {
            _logger.LogWarning("ENTERED ADMIN HANDLER");

            var userId = context.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            var access = await _redisUtils.GetOrFetchAccess(userId);
            if (access == null) return;

            if (access.IsAdmin)
                context.Succeed(requirement);
        }


    }
}
