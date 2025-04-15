using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.Misc;
using TicketingSys.Redis;
using TicketingSys.RoleHandling.Policies;
using TicketingSys.Settings;


namespace TicketingSys.RoleHandling.RoleHandlers
{
    public class RegularUserOnlyHandler : BaseRoleHandler<RegularUserOnlyRequirement>
    {
        public RegularUserOnlyHandler(ApplicationDbContext context, IRedisUtils redisUtils)
        : base(context, redisUtils) { }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RegularUserOnlyRequirement requirement)
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            var access = await _redisUtils.GetOrFetchAccess(userId);
            if (access == null) return;

            if (!access.IsAdmin && !access.HasDepartmentAccess)
                context.Succeed(requirement);
        }

    }
}
