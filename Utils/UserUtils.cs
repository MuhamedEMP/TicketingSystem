using System.Security.Claims;
using TicketingSys.Contracts.Misc;


namespace TicketingSys.Util
{

        public class UserUtils : IUserUtils
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            public UserUtils(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }

            public string? getUserId()
            { // sub is the user id
                var user = _httpContextAccessor.HttpContext?.User;
                return user?.FindFirst("sub")?.Value
                    ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }

}
