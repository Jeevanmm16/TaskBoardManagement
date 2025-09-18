using System.ComponentModel.DataAnnotations;

namespace TaskBoardManagement.Models.DTOs
{
    public class UpdateProjectDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string? Description { get; set; }
    }
}
