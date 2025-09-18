using TaskBoardManagement.Repositories;

namespace TaskBoardManagement.UnitofWork
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskItemRepository TaskItems { get; }
        ITaskAssignmentRepository TaskAssignments { get; }
        Task<int> CompleteAsync();
    }
}
