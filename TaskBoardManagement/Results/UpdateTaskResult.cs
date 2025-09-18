using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Results
{
    public class UpdateTaskResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public TaskItem? Task { get; set; }
    }

}
