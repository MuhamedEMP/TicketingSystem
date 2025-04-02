using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Controllers;
using TicketingSys.Dtos.UserDtos;
using Xunit;

namespace TicketingSys.Tests.SharedControllerTests
{
    public class ViewUserProfileTests
    {
        private readonly Mock<ISharedService> _sharedService = new();
        private readonly Mock<IUserUtils> _userUtils = new();
        private readonly Mock<IAttachmentService> _attachmentService = new();

        private SharedController CreateController() =>
            new SharedController(_sharedService.Object, _userUtils.Object, _attachmentService.Object);

        [Fact]
        public async Task ProfileById_ReturnsUser_WhenUserExists()
        {
            var userId = "456";
            var expectedDto = new ViewUserDto
            {
                firstName = "Jane",
                lastName = "Doe",
                fullName = "Jane Doe",
                email = "jane.doe@example.com",
                roles = new List<string> { "hr" }
            };

            _sharedService.Setup(s => s.getUserById(userId)).ReturnsAsync(expectedDto);

            var controller = CreateController();

            var result = await controller.getProfileById(userId);

            var response = Assert.IsType<ActionResult<ViewUserDto>>(result);
            var actualDto = Assert.IsType<ViewUserDto>(response.Value);
            Assert.Equal("Jane Doe", actualDto.fullName);
            Assert.Equal("jane.doe@example.com", actualDto.email);
            Assert.Contains("hr", actualDto.roles);
        }

        [Fact]
        public async Task ProfileById_Returns404_WhenUserNotFound()
        {
            var userId = "999";

            _sharedService.Setup(s => s.getUserById(userId)).ReturnsAsync((ViewUserDto)null);

            var controller = CreateController();

            var result = await controller.getProfileById(userId);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFound.StatusCode);

            var responseBody = notFound.Value?.ToString();
            Assert.Contains("User profile not found", responseBody);
        }
    }
}
