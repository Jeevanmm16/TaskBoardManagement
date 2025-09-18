using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Repositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(User user, string roleName);
    }
}
