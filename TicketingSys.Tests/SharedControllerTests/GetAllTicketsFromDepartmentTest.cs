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
using TicketingSys.Dtos.TicketDtos;

namespace TicketingSys.Tests.SharedControllerTests
{
    public class GetAllTicketsFromDepartmentTest
    {
        [Fact]
        public async Task GetAllTickets_ReturnsTickets_WhenTicketsExist()
        {
            // Arrange
            var mockSharedService = new Mock<ISharedService>();
            var mockUserUtils = new Mock<IUserUtils>();
            var mockAttachmentService = new Mock<IAttachmentService>();

            var userId = "123";
            mockUserUtils.Setup(x => x.getUserId()).Returns(userId);

            var tickets = new List<ViewTicketDto>
            {
                new ViewTicketDto { Id = 1, Title = "Issue A" },
                new ViewTicketDto { Id = 2, Title = "Issue B" }
            };

            mockSharedService.Setup(s => s.getAllTicketsFromMyDepartment(userId)).ReturnsAsync(tickets);

            var controller = new SharedController(mockSharedService.Object, mockUserUtils.Object, mockAttachmentService.Object);

            // Act
            var result = await controller.getAllTickets();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<ViewTicketDto>>(okResult.Value);
            Assert.Equal(2, returnedTickets.Count);
        }

        [Fact]
        public async Task GetAllTickets_Returns404_WhenNoTickets()
        {
            // Arrange
            var mockSharedService = new Mock<ISharedService>();
            var mockUserUtils = new Mock<IUserUtils>();
            var mockAttachmentService = new Mock<IAttachmentService>();

            var userId = "123";
            mockUserUtils.Setup(x => x.getUserId()).Returns(userId);
            mockSharedService.Setup(s => s.getAllTicketsFromMyDepartment(userId)).ReturnsAsync(new List<ViewTicketDto>());

            var controller = new SharedController(mockSharedService.Object, mockUserUtils.Object, mockAttachmentService.Object);

            // Act
            var result = await controller.getAllTickets();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFound.StatusCode);
        }
    }
}
