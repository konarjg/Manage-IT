﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

internal class TaskListContext : DbContext
{
    public DbSet<TaskList> TaskLists { get; set; }
}

[Table("TaskLists")]
public class TaskList
{
    [Key]
    public int TaskListId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [ForeignKey("ProjectId")]
    public int ProjectId { get; set; }
}