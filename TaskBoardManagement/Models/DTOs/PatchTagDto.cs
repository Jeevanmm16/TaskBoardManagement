using System.ComponentModel.DataAnnotations;

namespace TaskBoardManagement.Models.DTOs
{
    public class PatchTagDto
    {
        public string? Name { get; set; }

        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Invalid color hex.")]
        public string? ColorHex { get; set; } 
    }
}
