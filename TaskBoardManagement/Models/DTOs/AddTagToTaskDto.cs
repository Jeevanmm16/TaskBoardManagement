using System.ComponentModel.DataAnnotations;

namespace TaskBoardManagement.Models.DTOs
{
    public class AddTagToTaskDto
    {
        [Required]
        public Guid TagId { get; set; }
    }
}

