namespace TaskBoardManagement.Models.DTOs
{
    public class TaskAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid TaskItemId { get; set; }
        public string TaskTitle { get; set; } = default!;
        public Guid AssignedToUserId { get; set; }
        public string AssignedToEmail { get; set; } = default!;
        public Guid AssignedByUserId { get; set; }
        public string AssignedByEmail { get; set; } = default!;
        public DateTime AssignedAt { get; set; }
        public string? Comment { get; set; }
    }
}
