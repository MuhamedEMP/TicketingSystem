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
using TicketingSys.Models;

namespace TicketingSys.Tests.SharedControllerTests
{
    public class GetTicketByIdTests
    {
        [Fact]
        public async Task Get_Ticket_When_Exists_And_User_In_Same_Dept()
        {
            int ticketId = 1;
            var roles = new List<string> { "it" };

            var ticket = new Ticket
            {
                Id = ticketId,
                Title = "Printer Issue",
                Description = "Printer not working",
                Department = new Department { Name = "IT" }
            };

            var mockUserUtils = new Mock<IUserUtils>();
            var mockAttachmentService = new Mock<IAttachmentService>();
            mockUserUtils.Setup(x => x.getUserRoles()).ReturnsAsync(roles);

            var mockSharedService = new Mock<ISharedService>();
            mockSharedService.Setup(x => x.getTicketById(ticketId)).ReturnsAsync(ticket);

            var controller = new SharedController(mockSharedService.Object, mockUserUtils.Object, mockAttachmentService.Object);

            var result = await controller.getTicketById(ticketId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<ViewTicketDto>(okResult.Value);
            Assert.Equal(ticketId, returnedDto.Id);
        }

        [Fact]
        public async Task Get_Ticket_When_Exists_And_User_Is_Admin()
        {
            // Arrange
            int ticketId = 2;
            var roles = new List<string> { "admin" };

            var ticket = new Ticket
            {
                Id = ticketId,
                Title = "System Crash",
                Description = "Blue screen on boot",
                Department = new Department { Name = "HR" }
            };

            var mockUserUtils = new Mock<IUserUtils>();
            var mockAttachmentService = new Mock<IAttachmentService>();
            mockUserUtils.Setup(x => x.getUserRoles()).ReturnsAsync(roles);

            var mockSharedService = new Mock<ISharedService>();
            mockSharedService.Setup(x => x.getTicketById(ticketId)).ReturnsAsync(ticket);

            var controller = new SharedController(mockSharedService.Object, mockUserUtils.Object, mockAttachmentService.Object);

            // Act
            var result = await controller.getTicketById(ticketId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<ViewTicketDto>(okResult.Value);
            Assert.Equal(ticketId, returnedDto.Id);
        }

        [Fact]
        public async Task Get_Ticket_When_Not_Found_Returns_NotFound()
        {
            // Arrange
            int ticketId = 3;
            var roles = new List<string> { "it" };

            var mockUserUtils = new Mock<IUserUtils>();
            var mockAttachmentService = new Mock<IAttachmentService>();
            mockUserUtils.Setup(x => x.getUserRoles()).ReturnsAsync(roles);

            var mockSharedService = new Mock<ISharedService>();
            mockSharedService.Setup(x => x.getTicketById(ticketId)).ReturnsAsync((Ticket)null);

            var controller = new SharedController(mockSharedService.Object, mockUserUtils.Object, mockAttachmentService.Object);

            // Act
            var result = await controller.getTicketById(ticketId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Ticket not found", notFoundResult.Value);
        }

        [Fact]
        public async Task Get_Ticket_When_User_Not_Admin_Or_Same_Dept_Returns_Forbidden()
        {
            // Arrange
            int ticketId = 4;
            var roles = new List<string> { "it" };

            var ticket = new Ticket
            {
                Id = ticketId,
                Title = "Payroll Error",
                Description = "Salary miscalculation",
                Department = new Department { Name = "HR" }
            };

            var mockUserUtils = new Mock<IUserUtils>();
            var mockAttachmentService = new Mock<IAttachmentService>();
            mockUserUtils.Setup(x => x.getUserRoles()).ReturnsAsync(roles);

            var mockSharedService = new Mock<ISharedService>();
            mockSharedService.Setup(x => x.getTicketById(ticketId)).ReturnsAsync(ticket);

            var controller = new SharedController(mockSharedService.Object, mockUserUtils.Object, mockAttachmentService.Object);

            // Act
            var result = await controller.getTicketById(ticketId);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }
    }
}
