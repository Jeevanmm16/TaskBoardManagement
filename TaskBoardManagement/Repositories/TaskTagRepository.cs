using Microsoft.EntityFrameworkCore;
using TaskBoardManagement.Data;
using TaskBoardManagement.ExceptionMiddleware;
using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
    public class TaskTagRepository : ITaskTagRepository
    {
        private readonly AppDbContext _context;
        public TaskTagRepository(AppDbContext context) => _context = context;

        // GET all tags for a task
        public async Task<IEnumerable<TaskTag>> GetTagsForTaskAsync(Guid taskId)
        {
            var taskExists = await _context.TaskItems.AnyAsync(t => t.Id == taskId);
            if (!taskExists)
                throw new EntityNotFoundException("Task", taskId);

            return await _context.TaskTags
                .Include(tt => tt.Tag)
                .Where(tt => tt.TaskItemId == taskId)
                .AsNoTracking()
                .ToListAsync();
        }

        // GET single tag for a task
        public async Task<TaskTag> GetByIdAsync(Guid taskId, Guid tagId)
        {
            var taskTag = await _context.TaskTags
                .Include(tt => tt.Tag)
                .FirstOrDefaultAsync(tt => tt.TaskItemId == taskId && tt.TagId == tagId);

            if (taskTag == null)
                throw new EntityNotFoundException("TaskTag", tagId);

            return taskTag;
        }

        // POST - Add tag to a task
        public async Task<TaskTag> AddTagToTaskAsync(Guid taskId, Guid tagId)

        {
            var task = await _context.TaskItems.FindAsync(taskId);
            if (task == null)
                throw new EntityNotFoundException("Task", taskId);

            var tag = await _context.Tags.FindAsync(tagId);
            if (tag == null)
                throw new EntityNotFoundException("Tag", tagId);

            var exists = await _context.TaskTags
                .AnyAsync(tt => tt.TaskItemId == taskId && tt.TagId == tagId);
            if (exists)
                throw new ConflictException("TaskTag", tagId);

            var taskTag = new TaskTag { TaskItemId = taskId, TagId = tagId };
            _context.TaskTags.Add(taskTag);
            await _context.SaveChangesAsync();

            return taskTag;
        }

        // DELETE - Remove tag from task
        public async Task<Tag> RemoveTagFromTaskAsync(Guid taskId, Guid tagId)
        {
            var taskTag = await _context.TaskTags
                .Include(tt => tt.Tag)
                .FirstOrDefaultAsync(tt => tt.TaskItemId == taskId && tt.TagId == tagId);

            if (taskTag == null)
                throw new EntityNotFoundException("TaskTag", tagId);

            _context.TaskTags.Remove(taskTag);
            await _context.SaveChangesAsync();

            return taskTag.Tag;
        }

        // PUT - Replace old tag with new tag
        public async Task<Tag> ReplaceTagForTaskAsync(Guid taskId, Guid oldTagId, Guid newTagId)
        {
            var taskTag = await _context.TaskTags
                .FirstOrDefaultAsync(tt => tt.TaskItemId == taskId && tt.TagId == oldTagId);

            if (taskTag == null)
                throw new EntityNotFoundException("TaskTag", oldTagId);

            var newTag = await _context.Tags.FindAsync(newTagId);
            if (newTag == null)
                throw new EntityNotFoundException("Tag", newTagId);

            var duplicate = await _context.TaskTags
                .AnyAsync(tt => tt.TaskItemId == taskId && tt.TagId == newTagId);
            if (duplicate)
                throw new ConflictException("TaskTag", newTagId);

            taskTag.TagId = newTagId;
            await _context.SaveChangesAsync();

            return newTag;
        }

        // PATCH - Partial update (only supports TagId change)
        public async Task<TaskTag> PatchTagForTaskAsync(Guid taskId, Guid tagId, Guid? newTaskId, Guid? newTagId)
        {
            var taskTag = await _context.TaskTags
                .FirstOrDefaultAsync(tt => tt.TaskItemId == taskId && tt.TagId == tagId);

            if (taskTag == null)
                throw new EntityNotFoundException("TaskTag", tagId);

            // If no changes requested, just return the existing entity
            if (!newTaskId.HasValue && !newTagId.HasValue)
                return taskTag;

            // Determine new IDs (fallback to old ones if null)
            var updatedTaskId = newTaskId ?? taskTag.TaskItemId;
            var updatedTagId = newTagId ?? taskTag.TagId;

            // If both IDs are unchanged, just return existing entity
            if (updatedTaskId == taskTag.TaskItemId && updatedTagId == taskTag.TagId)
                return taskTag;

            // Ensure new Task exists (if TaskId is changed)
            if (newTaskId.HasValue && updatedTaskId != taskTag.TaskItemId)
            {
                var taskExists = await _context.TaskItems.AnyAsync(t => t.Id == updatedTaskId);
                if (!taskExists)
                    throw new EntityNotFoundException("Task", updatedTaskId);
            }

            // Ensure new Tag exists (if TagId is changed)
            if (newTagId.HasValue && updatedTagId != taskTag.TagId)
            {
                var tagExists = await _context.Tags.AnyAsync(t => t.Id == updatedTagId);
                if (!tagExists)
                    throw new EntityNotFoundException("Tag", updatedTagId);
            }

            // Ensure no duplicate relation already exists
            var duplicateExists = await _context.TaskTags
                .AnyAsync(tt => tt.TaskItemId == updatedTaskId && tt.TagId == updatedTagId);
            if (duplicateExists)
                throw new ConflictException("TaskTag", updatedTagId);

            // Remove old entry
            _context.TaskTags.Remove(taskTag);

            // Add new entry
            var newTaskTag = new TaskTag
            {
                TaskItemId = updatedTaskId,
                TagId = updatedTagId
            };

            _context.TaskTags.Add(newTaskTag);

            await _context.SaveChangesAsync();
            return newTaskTag;
        }

    }
}
