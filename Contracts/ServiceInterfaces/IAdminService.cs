using TicketingSys.Dtos.UserDtos;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IAdminService
    {
        Task<List<ViewUserDto>?> getAllUsers();
    }
}
