using System.ComponentModel.DataAnnotations;
using TaskBoardManagement.Models.Domain;
using TaskStatus = TaskBoardManagement.Models.Domain.TaskStatus;

public class PatchTaskItemDto
{
    [StringLength(100)]
    public string? Title { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public Guid? AssignedToUserId { get; set; }

    [EnumDataType(typeof(TaskStatus))]
    public TaskStatus? Status { get; set; }

    [EnumDataType(typeof(TaskPriority))]
    public TaskPriority? Priority { get; set; }


    
}