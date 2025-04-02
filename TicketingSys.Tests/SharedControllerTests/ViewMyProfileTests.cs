using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Controllers;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;

namespace TicketingSys.Tests.SharedControllerTests
{
    public class ViewMyProfileTests
    {
        private readonly Mock<ISharedService> _sharedService = new();
        private readonly Mock<IUserUtils> _userUtils = new();
        private readonly Mock<IAttachmentService> _attachmentService = new();

        private SharedController CreateController() =>
            new SharedController(_sharedService.Object, _userUtils.Object, _attachmentService.Object);

        [Fact]
        public async Task MyProfile_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = "123";

            var expectedDto = new ViewUserDto
            {
                firstName = "Admin",
                lastName = "User",
                fullName = "Admin User",
                email = "admin@test.com",
                roles = new List<string> { "admin" }
            };

            _userUtils.Setup(u => u.getUserId()).Returns(userId);
            _sharedService.Setup(s => s.getUserById(userId)).ReturnsAsync(expectedDto);

            var controller = CreateController();

            // Act
            var result = await controller.myProfile();

            // Assert
            var response = Assert.IsType<ActionResult<ViewUserDto>>(result);
            var actualDto = Assert.IsType<ViewUserDto>(response.Value);
            Assert.Equal("Admin User", actualDto.fullName);
            Assert.Equal("admin@test.com", actualDto.email);
            Assert.Contains("admin", actualDto.roles);
        }




        [Fact]
        public async Task MyProfile_Returns404_WhenUserNotFound()
        {
            // Arrange
            var userId = "123";
            _userUtils.Setup(u => u.getUserId()).Returns(userId);
            var controller = CreateController();

            // Act
            var result = await controller.myProfile();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFound.StatusCode);
        }
    }

}
