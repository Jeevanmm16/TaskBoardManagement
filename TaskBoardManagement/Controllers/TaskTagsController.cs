using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskBoardManagement.Models.DTOs;
using TaskBoardManagement.Repositories;

namespace TaskBoardManagement.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:guid}/tags")]
    public class TaskTagsController : ControllerBase
    {
        private readonly ITaskTagRepository _taskTagRepo;
        private readonly IMapper _mapper;

        public TaskTagsController(ITaskTagRepository taskTagRepo, IMapper mapper)
        {
            _taskTagRepo = taskTagRepo;
            _mapper = mapper;
        }

        // GET: api/tasks/{taskId}/tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskTagDto>>> GetTagsForTask(Guid taskId)
        {
            var tags = await _taskTagRepo.GetTagsForTaskAsync(taskId);
            return Ok(_mapper.Map<IEnumerable<TaskTagDto>>(tags));
        }

        // GET: api/tasks/{taskId}/tags/{tagId}
        [HttpGet("{tagId:guid}")]
        public async Task<ActionResult<TaskTagDto>> GetById(Guid taskId, Guid tagId)
        {
            var tag = await _taskTagRepo.GetByIdAsync(taskId, tagId);
            return Ok(_mapper.Map<TaskTagDto>(tag));
        }

        // POST: api/tasks/{taskId}/tags
        [HttpPost]
        public async Task<ActionResult<TaskTagDto>> AddTagToTask(Guid taskId, CreateTaskTagDto dto)
        {
            var created = await _taskTagRepo.AddTagToTaskAsync(taskId, dto.TagId);
            return CreatedAtAction(nameof(GetById), new { taskId, tagId = created.TagId },
                _mapper.Map<TaskTagDto>(created));
        }

        // PUT: api/tasks/{taskId}/tags/{tagId}
        [HttpPut("{tagId:guid}")]
        public async Task<ActionResult<TaskTagDto>> ReplaceTagForTask(Guid taskId, Guid tagId, UpdateTaskTagDto dto)
        {
            var updated = await _taskTagRepo.ReplaceTagForTaskAsync(taskId, tagId, dto.TagId);
            return Ok(_mapper.Map<TaskTagDto>(updated));
        }

        // PATCH: api/tasks/{taskId}/tags/{tagId}
        [HttpPatch("{tagId:guid}")]
        public async Task<ActionResult<TaskTagDto>> PatchTagForTask(Guid taskId, Guid tagId, PatchTaskTagDto dto)
        {
            var updated = await _taskTagRepo.PatchTagForTaskAsync(taskId, tagId, dto.TaskItemId, dto.TagId);
            return Ok(_mapper.Map<TaskTagDto>(updated));
        }

        // DELETE: api/tasks/{taskId}/tags/{tagId}
        [HttpDelete("{tagId:guid}")]
        public async Task<IActionResult> RemoveTagFromTask(Guid taskId, Guid tagId)
        {
            var removed = await _taskTagRepo.RemoveTagFromTaskAsync(taskId, tagId);
            return Ok(new { message = $"Tag '{removed.Name}' removed from task successfully" });
        }
    }
}
