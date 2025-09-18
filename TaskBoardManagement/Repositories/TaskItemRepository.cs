using Microsoft.EntityFrameworkCore;
using TaskBoardManagement.Data;
using TaskBoardManagement.ExceptionMiddleware;
using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Middleware;

namespace TaskBoardManagement.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly AppDbContext _context;

        public TaskItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Users.AnyAsync(t => t.Id == id);
        }
        public async Task<bool> Projectexist(Guid id)
        {
            return await _context.Projects.AnyAsync(t => t.Id == id);
        }
        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
           
            return await _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TaskItem task)
        {
            // ✅ Check Project existence
          

            // ✅ Add task after validations
            await _context.TaskItems.AddAsync(task);
        }

        public async Task Update(TaskItem task)
        {
           
            _context.TaskItems.Update(task);
        }

        public void Delete(TaskItem task)
        {
            _context.TaskItems.Remove(task);
        }
    }
}
