using Microsoft.EntityFrameworkCore;
using TaskBoardManagement.Data;
using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
    public class TaskAssignmentRepository : ITaskAssignmentRepository
    {
        private readonly AppDbContext _context;
        public TaskAssignmentRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(TaskAssignment assignment)
        {
            await _context.TaskAssignments.AddAsync(assignment);
        }

        public async Task<IEnumerable<TaskAssignment>> GetAllAsync()
        {
            return await _context.TaskAssignments
                .Include(a => a.TaskItem)
                .Include(a => a.AssignedToUser)
                .Include(a => a.AssignedByUser)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskAssignment>> GetByTaskIdAsync(Guid taskId)
        {
            return await _context.TaskAssignments
                .Include(a => a.TaskItem)
                .Include(a => a.AssignedToUser)
                .Include(a => a.AssignedByUser)
                .Where(a => a.TaskItemId == taskId)
                .AsNoTracking()
                .ToListAsync();
        }
    
   }
}
