using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
    public interface IProjectMemberRepository
    {
        Task<IEnumerable<User>> GetMembersAsync(Guid projectId);
        Task<ProjectMember> AddMemberAsync(Guid projectId, Guid userId);
        Task<ProjectMember> RemoveMemberAsync(Guid projectId, Guid userId);
    }
}
