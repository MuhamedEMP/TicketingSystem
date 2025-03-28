using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface ISharedService
    {
        Task<ViewUserDto?> getUserById(string id); 

        Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId);

        Task<List<Ticket>?> getAllTicketByUserId(string userId);

        Task<Response> AddResponse(NewResponseDto dto, string userId);

        Task<Ticket?> getTicketById(int ticketId);

        Task<List<ViewResponseDto>> getResponsesSentByUser(string userId); 

        Task<ViewResponseDto?> getSentResponseByUserIdAndResponseId(string userId, int responseId);

        Task<List<ViewTicketDto>?> getAllTicketsFromMyDepartment(string userId);

        Task<List<ViewTicketDto>?> queryAlLTicketsFromMyDepartment(string currentUserId, SharedTicketQueryParamsDto query);
    }
}
