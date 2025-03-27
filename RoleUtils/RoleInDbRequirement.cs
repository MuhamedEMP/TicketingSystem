using Microsoft.AspNetCore.Authorization;

namespace TicketingSys.RoleUtils
{
    public class RoleInDbRequirement : IAuthorizationRequirement
    {
    
        public string requiredRole {  get; set; }

        public RoleInDbRequirement(string role)
        {
            requiredRole = role;
        }
    }
}
