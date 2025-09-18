using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Results;

namespace TaskBoardManagement.Repositories
{
   
        public interface ITaskItemRepository
        {
            // ✅ Create
            Task AddAsync(TaskItem task);

            // ✅ Read
            Task<IEnumerable<TaskItem>> GetAllAsync();
            Task<TaskItem?> GetByIdAsync(Guid id);

            // ✅ Update
            Task Update(TaskItem task);

        Task<bool> ExistsAsync(Guid id);
        Task<bool> Projectexist(Guid id);

            // ✅ Delete
            void Delete(TaskItem task);
        }
    }



