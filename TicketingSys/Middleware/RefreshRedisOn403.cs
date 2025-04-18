using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.Misc;
using TicketingSys.Redis;
using TicketingSys.Settings;

namespace TicketingSys.Middleware
{
    // had to use ai here - might cause problems ?
    public class RefreshRedisOn403 : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
        private readonly ILogger<RefreshRedisOn403> _logger;

        public RefreshRedisOn403(ILogger<RefreshRedisOn403> logger)
        {
            _logger = logger;
        }

        // before sending 403 query the postgres db and write that data to redis and retry request
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (!authorizeResult.Succeeded &&
            context.User.Identity?.IsAuthenticated == true &&
            !context.Items.ContainsKey(RetryKeys.RedisRefreshed))
            {
                _logger.LogInformation("Activated 403 middleware in RefreshRedisOn403");

                var userUtils = context.RequestServices.GetRequiredService<IUserUtils>();
                var userId = userUtils.getUserIdOr401();

                var cacheService = context.RequestServices.GetRequiredService<IUserAccessCacheService>();
                var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();

                var user = await dbContext.Users
                    .Include(u => u.DepartmentAccesses)
                    .FirstOrDefaultAsync(u => u.userId == userId);

                if (user != null)
                {
                    await cacheService.SetUserAccessAsync(userId, user.IsAdmin, user.DepartmentAccesses.Any());
                    _logger.LogWarning("REFRESHED Redis user access for {userId} after 403", userId);
                }

                context.Items[RetryKeys.RedisRefreshed] = true;

                // ✅ Re-evaluate authorization using the policy
                var authService = context.RequestServices.GetRequiredService<IAuthorizationService>();
                var authResult = await authService.AuthorizeAsync(context.User, null, policy);

                if (authResult.Succeeded)
                {
                    _logger.LogInformation("✅ Authorization passed after Redis refresh");
                    await next(context); // Now safe to continue
                    return;
                }

                _logger.LogWarning("❌ Still unauthorized after Redis refresh");
            }
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}

