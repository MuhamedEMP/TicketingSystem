using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TicketingSys.Tests.AuthTests
{
    public class RolePolicyTests
    {
        [Theory]
        [InlineData("/Test/admin", new[] { "admin" }, HttpStatusCode.OK)]
        [InlineData("/Test/hr", new[] { "hr" }, HttpStatusCode.OK)]
        [InlineData("/Test/it", new[] { "it" }, HttpStatusCode.OK)]
        [InlineData("/Test/user", new[] { "user" }, HttpStatusCode.OK)]
        [InlineData("/Test/adminhrit", new[] { "admin" }, HttpStatusCode.OK)]
        [InlineData("/Test/adminhrit", new[] { "hr" }, HttpStatusCode.OK)]
        [InlineData("/Test/adminhrit", new[] { "it" }, HttpStatusCode.OK)]
        [InlineData("/Test/hrorit", new[] { "hr" }, HttpStatusCode.OK)]
        [InlineData("/Test/hrorit", new[] { "it" }, HttpStatusCode.OK)]
        [InlineData("/Test/hroradmin", new[] { "hr" }, HttpStatusCode.OK)]
        [InlineData("/Test/hroradmin", new[] { "admin" }, HttpStatusCode.OK)]
        [InlineData("/Test/itoradmin", new[] { "it" }, HttpStatusCode.OK)]
        [InlineData("/Test/itoradmin", new[] { "admin" }, HttpStatusCode.OK)]
        [InlineData("/Test/hrorit_duplicate", new[] { "hr" }, HttpStatusCode.OK)]
        [InlineData("/Test/hrorit_duplicate", new[] { "it" }, HttpStatusCode.OK)]
        [InlineData("/Test/allroles", new[] { "admin" }, HttpStatusCode.OK)]
        [InlineData("/Test/allroles", new[] { "hr" }, HttpStatusCode.OK)]
        [InlineData("/Test/allroles", new[] { "it" }, HttpStatusCode.OK)]
        [InlineData("/Test/allroles", new[] { "user" }, HttpStatusCode.OK)]

        // negative tests
        [InlineData("/Test/admin", new[] { "hr" }, HttpStatusCode.Forbidden)]
        [InlineData("/Test/hr", new[] { "admin" }, HttpStatusCode.Forbidden)]
        [InlineData("/Test/it", new[] { "user" }, HttpStatusCode.Forbidden)]
        [InlineData("/Test/hrorit", new[] { "user" }, HttpStatusCode.Forbidden)]
        [InlineData("/Test/hroradmin", new[] { "it" }, HttpStatusCode.Forbidden)]
        [InlineData("/Test/allroles", new[] { "guest" }, HttpStatusCode.Forbidden)]
        public async Task Endpoint_Access_By_RolePolicy(string url, string[] roles, HttpStatusCode expectedStatus)
        {
            var factory = new CustomWebAppFactory(roles);
            var client = factory.CreateClient();

            var response = await client.GetAsync(url);

            Assert.Equal(expectedStatus, response.StatusCode);
        }
    }
}
