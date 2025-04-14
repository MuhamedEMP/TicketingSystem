using TicketingSys.Dtos.CategoryDtos;
using TicketingSys.Dtos.DepartmentDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IAdminService
    {
        Task<List<ViewUserDto>?> getAllUsers();

        Task<List<ViewUserDto>> queryUsers(UserQueryParamsDto queryParams);

        Task<ViewUserDto?> getUserById(string userId);

        Task<ViewUserDto?> changeRole(ChangeRoleDto dto, string userId);

        Task addDepartment(string name);

        Task addCategory(NewTicketCategoryDto category);

        Task<IEnumerable<ViewTicketCategoryDto>> getAllCategories();

        Task<bool> deleteCategoryById(int id);

        Task<bool> deleteDepartmentById(int id);

    }
}
