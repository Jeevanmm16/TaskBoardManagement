namespace TaskBoardManagement.Models.DTOs
{
    public class ProjectMemberDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = default!;
        public string RoleName { get; set; } = default!;
    }
}
