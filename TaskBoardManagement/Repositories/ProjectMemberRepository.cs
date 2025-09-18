using Microsoft.EntityFrameworkCore;
using TaskBoardManagement.Data;
using TaskBoardManagement.ExceptionMiddleware;
using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly AppDbContext _context;
        public ProjectMemberRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<User>> GetMembersAsync(Guid projectId)
        {
            // ✅ First, check if the project exists
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
            {
                throw new ProjectNotFoundException(projectId);
            }

            // ✅ If project exists, fetch members
            return await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.User)
                    .ThenInclude(u => u.Role)
                .Select(pm => pm.User)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<ProjectMember> AddMemberAsync(Guid projectId, Guid userId)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                throw new ProjectNotFoundException(projectId);

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                throw new UserNotFoundException(userId);

            var exists = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
            if (exists)
                throw new MemberAlreadyExistsException(userId, projectId);

            var member = new ProjectMember { ProjectId = projectId, UserId = userId };
            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();

            return member;
        }

        public async Task<ProjectMember> RemoveMemberAsync(Guid projectId, Guid userId)
        {
            // ✅ Check if project exists
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                throw new ProjectNotFoundException(projectId);

            // ✅ Check if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                throw new UserNotFoundException(userId);

            // ✅ Check if the membership exists
            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (member == null)
                throw new MemberNotFoundException(userId, projectId);

            // ✅ Remove the member
            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();

            return member;
        }

    }
}
