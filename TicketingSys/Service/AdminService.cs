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
        ILogger<AdminService> _logger;

        public AdminService(ApplicationDbContext context, IUserAccessCacheService redisService,
                            ILogger<AdminService> logger)
        {
            _context = context;
            _redisService = redisService;
            _logger = logger;
        }


        public async Task<List<ViewUserDto>?> getAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            var allAccess = await _context.UserDepartmentAccess
                .Include(uda => uda.Department)
                .ToListAsync();

            var result = users.Select(user =>
            {
                var deptAccessList = allAccess
                    .Where(uda => uda.UserId == user.userId)
                    .ToList();

                return user.userModelToDto(deptAccessList);
            }).ToList();

            return result;
        }


        public async Task<ViewUserDto?> getUserById(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == id);
            if (user == null)  return null;

            var deptAccessList = await _context.UserDepartmentAccess
            .Include(uda => uda.Department)
            .Where(uda => uda.UserId == id)
            .ToListAsync();

            return user.userModelToDto(deptAccessList);
        }

        public async Task<ViewUserDto?> changeRole(ChangeRoleDto dto, string userId)
        {

            var user = await _context.Users
                .Include(u => u.DepartmentAccesses)
                .FirstOrDefaultAsync(u => u.userId == userId);

            if (user == null) return null;

            if (dto.isAdmin is not null)
                user.IsAdmin = dto.isAdmin.Value;

            var currentDepartmentAccesses = await _context.UserDepartmentAccess
                .Where(da => da.UserId == userId)
                .ToListAsync();

            if (dto.DepartmentIds is not null)
            {
                _context.UserDepartmentAccess.RemoveRange(user.DepartmentAccesses);

                var newAccesses = dto.DepartmentIds.Select(deptId => new UserDepartmentAccess
                {
                    UserId = userId,
                    DepartmentId = deptId
                }).ToList();

                await _context.UserDepartmentAccess.AddRangeAsync(newAccesses);
            }

            await _context.SaveChangesAsync();

            await _redisService.InvalidateUserAccessAsync(userId);

            var dtoResult = user.userModelToDto();

            return dtoResult;
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

            if (queryParams.isAdmin != null)
            {
                usersQuery = usersQuery.Where(u => u.IsAdmin == queryParams.isAdmin.Value);
            }

            var users = await usersQuery.ToListAsync();

            var allAccess = await _context.UserDepartmentAccess
            .Include(uda => uda.Department)
            .ToListAsync();

            if (queryParams.hasDepartments != null)
            {
                users = users.Where(user =>
                {
                    var hasDept = allAccess.Any(uda => uda.UserId == user.userId);
                    return queryParams.hasDepartments.Value ? hasDept : !hasDept;
                }).ToList();
            }

            return users.Select(user =>
            {
                var deptAccessList = allAccess
                    .Where(uda => uda.UserId == user.userId)
                    .ToList();

                return user.userModelToDto(deptAccessList);
            }).ToList();
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

            var isReferencedByTickets = await _context.Tickets.AnyAsync(t => t.DepartmentId == id);
            var isReferencedByCategoreis = await _context.TicketCategories.AnyAsync(c => c.DepartmentId == id);

            if (isReferencedByTickets || isReferencedByCategoreis)
                throw new CantDeleteDepartmentException();
            
            _context.Remove(dept);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
