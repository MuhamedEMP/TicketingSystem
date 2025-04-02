using Microsoft.AspNetCore.Authorization;

namespace TicketingSys.RoleUtils
{
    public class RoleInDbRequirement : IAuthorizationRequirement
    {

        public List<string> RequiredRoles { get; }

        public RoleInDbRequirement(params string[] roles)
        {
            RequiredRoles = roles.Select(r => r.ToLowerInvariant()).ToList();
        }
    }
}
