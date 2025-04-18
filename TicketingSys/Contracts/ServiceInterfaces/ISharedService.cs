using TicketingSys.Dtos.CategoryDtos;
using TicketingSys.Dtos.DepartmentDtos;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Enums;
using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface ISharedService
    {
        Task<ViewUserDto?> getUserById(string id); 

        Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId);

        Task<List<Ticket>?> getAllTicketByUserId(string userId);

        Task<Response?> AddResponse(NewResponseDto dto, string userId);

        Task<Ticket?> getTicketById(int ticketId); 

        Task<List<ViewResponseDto>> getResponsesSentByUser(string userId); 

        Task<ViewResponseDto?> getSentResponseByUserIdAndResponseId(string userId, int responseId);

        Task<List<ViewTicketDto>?> getAllTicketsFromMyDepartment(string userId);

        Task<List<ViewTicketDto>?> queryAlLTicketsFromMyDepartment(string currentUserId, SharedTicketQueryParamsDto query);

        Task<List<ViewTicketDto>?> getAllTicketsFromUserByDepartment(string userId);

        Task<ViewTicketDto?> changeTicketStatus(int ticketId, TicketStatusEnum status, string userId);

        Task<ViewTicketDto?> assignTicketToUser(int ticketId, string currentUserId);

        Task<List<ViewDepartmentDto>> getAllDepartments();

        Task<List<ViewDepartmentDto>> getMyAssignedDepartments(string userId);

        Task<ViewTicketCategoryDto?> getCategoryById(int categoryId);

        Task<ViewDepartmentDto?> getDepartmentById(int departmentId);
    }
}
