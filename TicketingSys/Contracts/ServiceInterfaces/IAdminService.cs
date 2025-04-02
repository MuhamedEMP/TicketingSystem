using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IAdminService
    {
        Task<List<ViewUserDto>?> getAllUsers();

        Task<ViewUserDto?> getUserById(string userId);

        Task<ViewUserDto?> changeRole(ChangeRoleDto dto);

        Task addDepartment(string name);

        Task addCategory(NewTicketCategoryDto category);

    }
}
