using Microsoft.EntityFrameworkCore;
using TaskBoardManagement.Data;
using TaskBoardManagement.ExceptionMiddleware;
using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Project> GetByIdAsync(Guid id)
        {
            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                throw new ProjectNotFoundException(id);

            return project;
        }
        public async Task<Project> AddAsync(Project project)
        {
            // ✅ Check if the OwnerId exists in Users table
            var ownerExists = await _context.Users.AnyAsync(u => u.Id == project.OwnerId);
            if (!ownerExists)
            {
                throw new UserNotFoundException(project.OwnerId);
            }

            // ✅ Ensure the same Owner doesn’t already own another project
            var alreadyOwner = await _context.Projects.AnyAsync(p => p.OwnerId == project.OwnerId);
            if (alreadyOwner)
            {
                throw new OwnerAlreadyExistsException(project.OwnerId);
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }



        public async Task<Project?> UpdateAsync(Project project)
        {
            if (!_context.Projects.Any(p => p.Id == project.Id))
            {
                throw new ProjectNotFoundException(project.Id);
            }

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> DeleteAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return null;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return project;
        }
        public async Task<bool> OwnerExistsAsync(Guid ownerId)
        {
            return await _context.Users.AnyAsync(u => u.Id == ownerId);
        }
    }

}
