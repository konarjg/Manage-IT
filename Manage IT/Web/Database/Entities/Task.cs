using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("Tasks")]
public class Task
{
    [Key]
    public long TaskId { get; set; }
    [ForeignKey("TaskListId")]
    public long TaskListId { get; set; }
    public DateTime Deadline { get; set; }
    public string Description { get; set; }
}