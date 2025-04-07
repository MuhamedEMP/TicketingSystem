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

        public async Task<ViewUserDto?> changeRole(ChangeRoleDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == dto.userId);
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
            if (category != null) return false;
            _context.Remove(category);
            await _context.SaveChangesAsync();
            return true;    
        }

    }
}
