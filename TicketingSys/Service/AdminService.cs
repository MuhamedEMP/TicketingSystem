using Microsoft.EntityFrameworkCore;
using System.Data;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.CategoryDtos;
using TicketingSys.Dtos.DepartmentDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Service
{
    public class AdminService : IAdminService
    {
        ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<ViewUserDto>?> getAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => u.userModelToDto()).ToList();
        }

        public async Task<ViewUserDto?> getUserById(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == id);
            if (user == null)  return null;
            return user.userModelToDto();
        }

        public async Task<ViewUserDto?> changeRole(ChangeRoleDto dto, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == userId);
            if (user == null) return null;

            var roles = dto.roles.Select(r => r.ToLower()).ToList();
            
            user.roles = roles;

            await _context.SaveChangesAsync();

            return user.userModelToDto();
        }

        public async Task addDepartment(string name)
        {
            var newDepartment = new Department
            {
                Name = name,
            };
            await _context.Departments.AddAsync(newDepartment);
            await _context.SaveChangesAsync();
        }

        public async Task addCategory(NewTicketCategoryDto category)
        {
            var newCategory = new TicketCategory
            {
                DepartmentId = category.DepartmentId,
                Name = category.Name,
                Description = category.Description
            };
            await _context.TicketCategories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ViewTicketCategoryDto>> getAllCategories()
        {
            var categories = await _context.TicketCategories
                .Include(c => c.Department)
                .ToListAsync(); // ✅ this must stay — executes the SQL

            return categories
                .Where(c => c != null && c.Department != null)
                .Select(c => c.modelToViewDto()); // ✅ no ToList() here
        }



        public async Task<bool> deleteCategoryById(int id)
        {
            var category = await _context.TicketCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return false;
            _context.Remove(category);
            await _context.SaveChangesAsync();
            return true;    
        }

        public async Task<List<ViewUserDto>> queryUsers(UserQueryParamsDto queryParams)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.firstName))
            {
                usersQuery = usersQuery.Where(u => u.firstName.ToLower().Contains(queryParams.firstName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.lastName))
            {
                usersQuery = usersQuery.Where(u => u.lastName.ToLower().Contains(queryParams.lastName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.fullName))
            {
                usersQuery = usersQuery.Where(u =>
                    (u.firstName + " " + u.lastName).ToLower().Contains(queryParams.fullName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.email))
            {
                usersQuery = usersQuery.Where(u => u.email.ToLower().Contains(queryParams.email.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.role))
            {
                var roleFilter = queryParams.role.ToLower();
                usersQuery = usersQuery.Where(u =>
                    u.roles.Any(r => r.ToLower().Contains(roleFilter)));
            }

            var users = await usersQuery.ToListAsync();

            return users.Select(u => u.userModelToDto()).ToList();
        }

        public async Task<bool> deleteDepartmentById(int id)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(c => c.Id == id);
            if (dept == null) return false;
            _context.Remove(dept);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
