using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TicketingSys.Models;
using TicketingSys.RoleUtils;
using TicketingSys.Settings;

namespace TicketingSys.Tests.AuthTests
{
    public class AdminInDbTest
    {
        [Fact]
        public async Task User_With_Admin_Role_Passes_AdminFromDb_Policy()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var dbContext = new ApplicationDbContext(options);

            var userId = "123";
            dbContext.Users.Add(new User
            {
                userId = userId,
                email = "admin@test.com",
                firstName = "Admin",
                lastName = "Test",
                fullName = "Admin Test",
                roles = new List<string> { "admin" } 
            });
            await dbContext.SaveChangesAsync();

            var claims = new List<Claim> { new Claim("sub", userId) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var requirement = new RoleInDbRequirement("admin");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new RoleInDbHandler(dbContext);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }
    }
}
