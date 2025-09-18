﻿using TaskBoardManagement.Models.Domain;

public class TaskTag
{
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = default!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}