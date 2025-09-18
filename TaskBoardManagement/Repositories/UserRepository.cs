using global::TaskBoardManagement.Data;
using global::TaskBoardManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;
using TaskBoardManagement.Data;
using TaskBoardManagement.Models;
namespace TaskBoardManagement.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<User> Users, int TotalItems, string? Error)> GetAllAsync(
             int page = 1,
             int pageSize = 10,
             string? sortBy = null,
             string sortOrder = "asc",
             string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .AsQueryable();

            string? error = null;

            // Searching (only by email allowed)
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                if (!search.Contains("@")) // simple validation
                {
                    error = "Invalid search term. Only email search is allowed.";
                    return (Enumerable.Empty<User>(), 0, error);
                }

                query = query.Where(u => u.Email.ToLower().Contains(search));
            }

            // Sorting (only by email allowed)
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.ToLower() != "email")
                {
                    error = "Invalid sortBy value. Only 'email' is allowed.";
                    return (Enumerable.Empty<User>(), 0, error);
                }

                query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email);
            }
            else
            {
                query = query.OrderBy(u => u.Email); // default ascending
            }

            // Total count for pagination
            var totalItems = await query.CountAsync();

            // Handle search not found
            if (!string.IsNullOrWhiteSpace(search) && totalItems == 0)
            {
                error = "No users found matching the search term.";
                return (Enumerable.Empty<User>(), 0, error);
            }

            // Pagination
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalItems, error);
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {

            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> AddAsync(User user)
        {
            // ✅ Ensure role exists before inserting
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == user.RoleId);
            if (!roleExists)
            {
                return null; // invalid RoleId
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.Users.Add(user);

            // ✅ Auto-create empty profile
            var profile = new UserProfile
            {
                UserId = user.Id,
                FullName = string.Empty,
                Phone = null,
                Address = null
            };
            _context.UserProfiles.Add(profile);

            await _context.SaveChangesAsync();
            return user;
        }



        public async Task<User?> UpdateAsync(User user)
        {
            if (!_context.Users.Any(u => u.Id == user.Id))
            {
                return null;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Updated DeleteAsync to return User instead of bool
        public async Task<User?> DeleteAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

      
    }

}
