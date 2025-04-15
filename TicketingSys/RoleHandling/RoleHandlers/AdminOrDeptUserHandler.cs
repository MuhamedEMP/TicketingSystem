using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using TicketingSys.Contracts.Misc;
using TicketingSys.RoleHandling.Policies;
using TicketingSys.Settings;

namespace TicketingSys.RoleHandling.RoleHandlers
{
    public class AdminOrDeptUserHandler : BaseRoleHandler<AdminOrDeptUserRequirement>
    {

        public AdminOrDeptUserHandler(ApplicationDbContext context, IRedisUtils redisUtils)
        : base(context, redisUtils) { }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrDeptUserRequirement requirement)
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            var access = await _redisUtils.GetOrFetchAccess(userId);
            if (access == null) return;

            if (access.IsAdmin || access.HasDepartmentAccess)
                context.Succeed(requirement);
        }


    }
   }
