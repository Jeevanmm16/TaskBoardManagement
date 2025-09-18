using System.ComponentModel.DataAnnotations;namespace TaskBoardManagement.Models.DTOs{    public class TagDto    {        public Guid Id { get; set; }        [Required]        public string Name { get; set; } = default!;

        [Required]        public string ColorHex { get; set; } = "#FFFFFF";    }}