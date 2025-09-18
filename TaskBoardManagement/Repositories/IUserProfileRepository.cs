using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
   
        public interface IUserProfileRepository
        {
            Task<UserProfile?> GetByUserIdAsync(Guid userId);
            Task<UserProfile?> UpdateAsync(UserProfile profile);
        }
    
}
