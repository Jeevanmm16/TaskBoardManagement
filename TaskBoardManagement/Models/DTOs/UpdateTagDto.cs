using System.ComponentModel.DataAnnotations;

namespace TaskBoardManagement.Models.DTOs
{
    public class UpdateTagDto
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = default!;

        [Required, RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Invalid color hex.")]
        public string ColorHex { get; set; } = "#FFFFFF";
    }
}
