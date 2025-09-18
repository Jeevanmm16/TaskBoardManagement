using Microsoft.EntityFrameworkCore;
using TaskBoardManagement.Data;
using TaskBoardManagement.ExceptionMiddleware;

namespace TaskBoardManagement.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;
        public TagRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
           return await _context.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
          var exist=  await _context.Tags.FindAsync(id);
            if (exist == null)
            {
                throw new EntityNotFoundException("Tag", id);
            }
            return exist;
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var existing = await _context.Tags.FindAsync(tag.Id);
            if (existing == null)
            {
                throw new EntityNotFoundException("Tag", tag.Id);
            }

            _context.Entry(existing).CurrentValues.SetValues(tag);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                throw new EntityNotFoundException("Tag", id);
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return tag;
        }
    }

}
