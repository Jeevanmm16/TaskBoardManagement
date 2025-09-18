using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
    public interface ITaskTagRepository
    {
        // GET all tags for a task
        Task<IEnumerable<TaskTag>> GetTagsForTaskAsync(Guid taskId);

        // GET single tag for a task
        Task<TaskTag> GetByIdAsync(Guid taskId, Guid tagId);

        // POST - Add a tag to task
        Task<TaskTag> AddTagToTaskAsync(Guid taskId, Guid tagId);

        // DELETE - Remove tag from task
        Task<Tag> RemoveTagFromTaskAsync(Guid taskId, Guid tagId);

        // PUT - Replace old tag with new tag
        Task<Tag> ReplaceTagForTaskAsync(Guid taskId, Guid oldTagId, Guid newTagId);

        // PATCH - Partial update (currently only supports changing TagId)
        Task<TaskTag> PatchTagForTaskAsync(Guid taskId, Guid tagId, Guid? newTaskId, Guid? newTagId);
    }
}
