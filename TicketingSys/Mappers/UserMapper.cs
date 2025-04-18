using TicketingSys.Dtos.DepartmentDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;

namespace TicketingSys.Mappers
{
    public static class UserMapper
    {
        public static ViewUserDto userModelToDto(this User userModel)
        {

            return new ViewUserDto
            {
                userId = userModel.userId,
                fullName = userModel.fullName,
                firstName = userModel.firstName,
                lastName = userModel.lastName,
                email = userModel.email,
                isAdmin = userModel.IsAdmin,
            };
            
        
        }

        public static ViewUserDto userModelToDto(this User userModel, List<UserDepartmentAccess> userDepartmentAccessList)
        {
            var deptAccessListDtos = userDepartmentAccessList.Select(da => new AccessibleDepartmentDto
            {
                Id = da.Id,
                Name = da.Department.Name,
            }).ToList();

            return new ViewUserDto
            {
                userId = userModel.userId,
                fullName = userModel.fullName,
                firstName = userModel.firstName,
                lastName = userModel.lastName,
                email = userModel.email,
                isAdmin = userModel.IsAdmin,
                accessibleDepartmentDtos = deptAccessListDtos
            };


        }
    }
}
