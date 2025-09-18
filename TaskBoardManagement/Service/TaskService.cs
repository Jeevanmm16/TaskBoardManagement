using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskBoardManagement.ExceptionMiddleware;
using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Models.DTOs;
using TaskBoardManagement.UnitofWork;

namespace TaskBoardManagement.Service
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ✅ Create
        public async Task<TaskItem?> CreateTaskAsync(CreateTaskItemDto dto, Guid createdByUserId)
        {

            var Projectexist = await _unitOfWork.TaskItems.Projectexist(dto.ProjectId);
            if (!Projectexist)
            {
                throw new EntityNotFoundException("Project", dto.ProjectId);
            }

            if (dto.AssignedToUserId.HasValue)
            {
                var userExists = await _unitOfWork.TaskItems.ExistsAsync(dto.AssignedToUserId.Value);
                if (!userExists)
                    throw new EntityNotFoundException("User", dto.AssignedToUserId.Value);
            }

            var task = _mapper.Map<TaskItem>(dto);
            task.Id = Guid.NewGuid();

            await _unitOfWork.TaskItems.AddAsync(task);

            if (dto.AssignedToUserId.HasValue)
            {
                var assignment = new TaskAssignment
                {
                    Id = Guid.NewGuid(),
                    TaskItemId = task.Id,
                    AssignedToUserId = dto.AssignedToUserId.Value,
                    AssignedByUserId = createdByUserId,
                    AssignedAt = DateTime.UtcNow,
                    Comment = "Initial assignment"
                };

                await _unitOfWork.TaskAssignments.AddAsync(assignment);
            }

            await _unitOfWork.CompleteAsync();
            return task;
        }

        // ✅ Get All
        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _unitOfWork.TaskItems.GetAllAsync();
        }

        // ✅ Get By Id
        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
            var existingTask = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (existingTask == null)
                throw new EntityNotFoundException("Task", id);

            return await _unitOfWork.TaskItems.GetByIdAsync(id);
        }

        // ✅ Update (PUT)
        public async Task<TaskItem?> UpdateAsync(Guid id, UpdateTaskItemDto dto)
        {
            var existingTask = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (existingTask == null)
                throw new EntityNotFoundException("Task", id);

            var Projectexist = await _unitOfWork.TaskItems.Projectexist(dto.ProjectId);
            if (!Projectexist)
            {
                throw new EntityNotFoundException("Project", dto.ProjectId);
            }

            if (dto.AssignedToUserId.HasValue)
            {
                var userExists = await _unitOfWork.TaskItems.ExistsAsync(dto.AssignedToUserId.Value);
                if (!userExists)
                    throw new EntityNotFoundException("User", dto.AssignedToUserId.Value);
            }

            _mapper.Map(dto, existingTask);

            _unitOfWork.TaskItems.Update(existingTask);
            await _unitOfWork.CompleteAsync();

            return existingTask;
        }

        // ✅ Patch (Partial Update)
        public async Task<TaskItem?> PatchAsync(Guid id, PatchTaskItemDto dto)
        {
            var existingTask = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (existingTask == null)
                throw new EntityNotFoundException("Task", id);

            // only update provided fields
            if (dto.Title != null) existingTask.Title = dto.Title;
            if (dto.Description != null) existingTask.Description = dto.Description;
            if (dto.Status.HasValue) existingTask.Status = dto.Status.Value;
            if (dto.Priority.HasValue) existingTask.Priority = dto.Priority.Value;
            if (dto.AssignedToUserId.HasValue)
            {
                var userExists = await _unitOfWork.TaskItems.ExistsAsync(dto.AssignedToUserId.Value);
                if (!userExists)
                    throw new EntityNotFoundException("User", dto.AssignedToUserId.Value);
            }
            existingTask.AssignedToUserId = dto.AssignedToUserId;

            _unitOfWork.TaskItems.Update(existingTask);
            await _unitOfWork.CompleteAsync();

            return existingTask;
        }

        // ✅ Delete
        public async Task<TaskItem?> DeleteAsync(Guid id)
        {
            var existingTask = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (existingTask == null)
                throw new EntityNotFoundException("Task", id);

            _unitOfWork.TaskItems.Delete(existingTask);
            await _unitOfWork.CompleteAsync();

            return existingTask;
        }
    }
}
