using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketingSys.Models;
using TicketingSys.Settings;
using Microsoft.AspNetCore.Hosting;
using TicketingSys.RoleUtils;

namespace TicketingSys.Tests
{
    public class CustomWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly List<string> _testRoles;

        public CustomWebAppFactory(string[] roles)
        {
            _testRoles = roles.ToList(); 
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove real DB context
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // ✅ Use a uniquely named in-memory DB per test to avoid key conflicts
                var dbName = $"TestDb_{Guid.NewGuid()}";

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                // Add fake authenticated user with sub claim
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                // Register the custom policy
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("AdminFromDb", policy =>
                        policy.Requirements.Add(new RoleInDbRequirement("admin")));
                });

                var sp = services.BuildServiceProvider();

                // ✅ Seed the test DB with a known user
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Clear any existing users (extra safety if reusing db name)
                db.Users.RemoveRange(db.Users);

                db.Users.Add(new User
                {
                    userId = "123",
                    email = "test@example.com",
                    firstName = "Test",
                    lastName = "User",
                    fullName = "Test User",
                    roles = _testRoles // ✅ dynamic roles passed from the test
                });
                db.SaveChanges();
            });
        }
    }

    // ✅ Fake auth handler for injecting a test ClaimsPrincipal
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim("sub", "123"),
                new Claim(ClaimTypes.Name, "Test Admin")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
