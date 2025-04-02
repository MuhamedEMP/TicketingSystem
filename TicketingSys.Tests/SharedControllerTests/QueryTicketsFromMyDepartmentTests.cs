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
    public class QueryTicketsFromMyDepartmentTests
    {
        [Fact]
        public async Task GetAllTicketsWithQuery_ReturnsResults_WhenMatchingTicketsFound()
        {
            var mockSharedService = new Mock<ISharedService>();
            var mockUserUtils = new Mock<IUserUtils>();
            var mockAttachmentService = new Mock<IAttachmentService>();

            var userId = "123";
            var query = new SharedTicketQueryParamsDto { Search = "Network" };

            var expectedResults = new List<ViewTicketDto>
            {
                new ViewTicketDto { Id = 1, Title = "Network issue" }
            };

            mockUserUtils.Setup(x => x.getUserId()).Returns(userId);
            mockSharedService.Setup(s => s.queryAlLTicketsFromMyDepartment(userId, query)).ReturnsAsync(expectedResults);

            var controller = new SharedController(mockSharedService.Object, mockUserUtils.Object, mockAttachmentService.Object);

            var result = await controller.GetAllTicketsWithQuery(query);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<List<ViewTicketDto>>(okResult.Value);
            Assert.Single(returned);
        }

        [Fact]
        public async Task GetAllTicketsWithQuery_Returns404_WhenNoMatches()
        {
            var mockSharedService = new Mock<ISharedService>();
            var mockUserUtils = new Mock<IUserUtils>();
            var mockAttachmentService = new Mock<IAttachmentService>();

            var userId = "123";
            var query = new SharedTicketQueryParamsDto { Search = "NonExistent" };

            mockUserUtils.Setup(x => x.getUserId()).Returns(userId);
            mockSharedService.Setup(s => s.queryAlLTicketsFromMyDepartment(userId, query)).ReturnsAsync(new List<ViewTicketDto>());

            var controller = new SharedController(mockSharedService.Object, mockUserUtils.Object, mockAttachmentService.Object);

            var result = await controller.GetAllTicketsWithQuery(query);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFound.StatusCode);
        }
    }
}
