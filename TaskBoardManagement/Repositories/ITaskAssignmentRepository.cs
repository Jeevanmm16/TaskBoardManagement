using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
    public interface ITaskAssignmentRepository
    {
        Task AddAsync(TaskAssignment assignment);
        Task<IEnumerable<TaskAssignment>> GetAllAsync();
        Task<IEnumerable<TaskAssignment>> GetByTaskIdAsync(Guid taskId);
    }
}
