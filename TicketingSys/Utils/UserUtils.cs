using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketingSys.Contracts.Misc;
using TicketingSys.Exceptions;
using TicketingSys.Models;
using TicketingSys.Settings;


namespace TicketingSys.Util
{

    public class UserUtils : IUserUtils
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        public UserUtils(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public string? getUserIdOr401()
        { // sub is the user id
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst("sub")?.Value
                      ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // handling the exception and returning 401 is handled in ExceptionHandlingMiddleware
            if (string.IsNullOrEmpty(userId))
                throw new NoUserIdInJwtException("User not authenticated.");

            return userId;
        }



        public async Task<bool> checkIfCategoryIsValid(int categoryId, int departmentId)
        {
            var category = await _context.TicketCategories.FindAsync(categoryId);

            if (category == null)
            {
                Console.WriteLine($"❌ Category with ID {categoryId} not found");
                return false;
            }

            Console.WriteLine($"✅ Found category {category.Name}, category.DepartmentId = {category.DepartmentId}, given departmentId = {departmentId}");

            if (category.DepartmentId != departmentId)
            {
                Console.WriteLine("❌ DepartmentId mismatch");
                return false;
            }

            return true;
        }

    }

}
