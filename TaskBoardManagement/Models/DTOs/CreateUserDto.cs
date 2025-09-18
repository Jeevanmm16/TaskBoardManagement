using System.ComponentModel.DataAnnotations;
using TaskBoardManagement.Enums;

namespace TaskBoardManagement.Models.DTOs

{
    public class CreateUserDto
    {
       
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; } = default!;

            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
            public string Password { get; set; } = default!;

            [Required(ErrorMessage = "Role is required")]
            public UserRole RoleId { get; set; }   // enum (Swagger dropdown)
        
    }

}
