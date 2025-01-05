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
    public string Name { get; set; }
    [ForeignKey("TaskListId")]
    public long TaskListId { get; set; }
    public DateTime Deadline { get; set; }
    public string Description { get; set; }

    public string DaysLeft 
    { 
        get
        {
            int diff = (int)((Deadline - DateTime.Now).TotalDays);

            if (diff < 0)
            {
                diff = 0;
            }

            return $"{diff}d left";
        }
    }
}