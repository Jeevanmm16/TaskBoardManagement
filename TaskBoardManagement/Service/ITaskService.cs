using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Models.DTOs;

namespace TaskBoardManagement.Service
{
    public interface ITaskService
    {
     
            Task<TaskItem?> CreateTaskAsync(CreateTaskItemDto dto, Guid createdByUserId);
            Task<IEnumerable<TaskItem>> GetAllAsync();
            Task<TaskItem?> GetByIdAsync(Guid id);
            Task<TaskItem?> UpdateAsync(Guid id, UpdateTaskItemDto dto);
            Task<TaskItem?> PatchAsync(Guid id, PatchTaskItemDto dto);
            Task<TaskItem?> DeleteAsync(Guid id);
        
    }
}
