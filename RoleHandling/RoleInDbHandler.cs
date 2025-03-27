using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketingSys.Settings;

namespace TicketingSys.RoleUtils
{
    public class RoleInDbHandler : AuthorizationHandler<RoleInDbRequirement>
    {
        private readonly ApplicationDbContext _db;

        public RoleInDbHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleInDbRequirement requirement)
        {
            var sub = context.User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(sub))
                throw new Exception("NO SUB IN JWT");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.userId == sub);
            if (user != null && user.roles.Contains(requirement.requiredRole))
            {
                context.Succeed(requirement);
            }
        }
    }
}
