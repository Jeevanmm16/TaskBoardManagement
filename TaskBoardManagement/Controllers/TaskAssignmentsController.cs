using Microsoft.AspNetCore.Mvc;
using TaskBoardManagement.Models.DTOs;
using TaskBoardManagement.Repositories;

namespace TaskBoardManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskAssignmentsController : ControllerBase
    {
        private readonly ITaskAssignmentRepository _repo;

        public TaskAssignmentsController(ITaskAssignmentRepository repo)
        {
            _repo = repo;
        }

        // ✅ Get all assignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskAssignmentDto>>> GetAll()
        {
            var assignments = await _repo.GetAllAsync();

            var dtos = assignments.Select(a => new TaskAssignmentDto
            {
                Id = a.Id,
                TaskItemId = a.TaskItemId,
                TaskTitle = a.TaskItem.Title,
                AssignedToUserId = a.AssignedToUserId,
                AssignedToEmail = a.AssignedToUser.Email,
                AssignedByUserId = a.AssignedByUserId,
                AssignedByEmail = a.AssignedByUser.Email,
                AssignedAt = a.AssignedAt,
                Comment = a.Comment
            });

            return Ok(dtos);
        }

        // ✅ Get history for specific Task
        [HttpGet("task/{taskId:guid}")]
        public async Task<ActionResult<IEnumerable<TaskAssignmentDto>>> GetByTask(Guid taskId)
        {
            var assignments = await _repo.GetByTaskIdAsync(taskId);

            var dtos = assignments.Select(a => new TaskAssignmentDto
            {
                Id = a.Id,
                TaskItemId = a.TaskItemId,
                TaskTitle = a.TaskItem.Title,
                AssignedToUserId = a.AssignedToUserId,
                AssignedToEmail = a.AssignedToUser.Email,
                AssignedByUserId = a.AssignedByUserId,
                AssignedByEmail = a.AssignedByUser.Email,
                AssignedAt = a.AssignedAt,
                Comment = a.Comment
            });

            return Ok(dtos);
        }
    }
}
