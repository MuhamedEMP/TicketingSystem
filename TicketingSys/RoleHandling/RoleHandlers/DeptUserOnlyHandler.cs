﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TicketingSys.Contracts.Misc;
using TicketingSys.Redis;
using TicketingSys.RoleHandling.Policies;
using TicketingSys.Settings;

namespace TicketingSys.RoleHandling.RoleHandlers
{
    public class DeptUserOnlyHandler: BaseRoleHandler<DeptUserOnlyRequirement>
    {
        public DeptUserOnlyHandler(ApplicationDbContext context, IRedisUtils redisUtils, ILogger<DeptUserOnlyHandler> logger)
        : base(context, redisUtils, logger) { }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DeptUserOnlyRequirement requirement)
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            var access = await _redisUtils.GetOrFetchAccess(userId);
            if (access == null) return;

            // use this if you want dept users unable to have admin roles

            //if (!access.IsAdmin && access.HasDepartmentAccess) 
            //    context.Succeed(requirement);


            // allows user to be admin and pass this policy if he has department access
            if (access.HasDepartmentAccess) 
                context.Succeed(requirement);
        }

    }
}
