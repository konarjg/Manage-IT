using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

public class TaskContext : DbContext
{
    public DbSet<Task> Tasks { get; set; }
}

[Table("Tasks")]
public class Task
{
    [Key]
    public int TaskId { get; set; }
    [ForeignKey("TaskListId")]
    public int TaskListId { get; set; }
    public DateTime Deadline { get; set; }
    public string Description { get; set; }
}