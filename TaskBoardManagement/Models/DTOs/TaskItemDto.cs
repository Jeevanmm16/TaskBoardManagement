using System.ComponentModel.DataAnnotations;
using TaskBoardManagement.Models.Domain;
using TaskStatus = TaskBoardManagement.Models.Domain.TaskStatus;

namespace TaskBoardManagement.Models.DTOs
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? AssignedToUserId { get; set; }
        

        [Required]
        public TaskStatus Status { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }
    }

}