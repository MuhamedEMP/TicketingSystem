using Microsoft.EntityFrameworkCore;
using System.Data;
using TicketingSys.Contracts.Misc;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.CategoryDtos;
using TicketingSys.Dtos.DepartmentDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Exceptions;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Redis;
using TicketingSys.Settings;

namespace TicketingSys.Service
{
    public class AdminService : IAdminService
    {
        ApplicationDbContext _context;
        IUserAccessCacheService _redisService;

        public AdminService(ApplicationDbContext context, IUserAccessCacheService redisService)
        {
            _context = context;
            _redisService = redisService;
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
            var user = await _context.Users.
                Include(u=> u.DepartmentAccesses).
                FirstOrDefaultAsync(u => u.userId == userId);

            if (user == null) return null;

            if (dto.isAdmin is not null) user.IsAdmin = dto.isAdmin.Value;

            var currentDepartmentAccesses = await _context.UserDepartmentAccess.AllAsync(da => da.UserId == userId);

            if (dto.DepartmentIds is not null)
            {
                _context.UserDepartmentAccess.RemoveRange(user.DepartmentAccesses);

                var newAccesses = dto.DepartmentIds.Select(deptId => new UserDepartmentAccess
                {
                    UserId = userId,
                    DepartmentId = deptId
                });

                await _context.UserDepartmentAccess.AddRangeAsync(newAccesses);
            }

            await _context.SaveChangesAsync();

            await _redisService.InvalidateUserAccessAsync(userId);

            return user.userModelToDto();
        }


        public async Task addDepartment(string name)
        {
            var exists = await _context.Departments.FirstOrDefaultAsync(d=> d.Name == name);
            if (exists != null)
                throw new UniqueConstraintFailedException("Can not create two departments with same name");

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
                .ToListAsync();

            return categories
                .Where(c => c != null && c.Department != null)
                .Select(c => c.modelToViewDto()); 
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


            var users = await usersQuery.ToListAsync();

            return users.Select(u => u.userModelToDto()).ToList();
        }


        public async Task<bool> deleteCategoryById(int id)
        {
            var category = await _context.TicketCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return false;

            bool isReferenced = await _context.Tickets.AnyAsync(t => t.CategoryId == id);

            if (isReferenced)
                throw new CantDeleteCategoryException("Cannot delete category with assigned tickets.");

            _context.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> deleteDepartmentById(int id)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(c => c.Id == id);
            if (dept == null) return false;

            var isReferenced = await _context.Tickets.AnyAsync(t => t.DepartmentId == id);
            if (isReferenced)
                throw new CantDeleteDepartmentException();
            
            _context.Remove(dept);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
