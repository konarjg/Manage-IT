using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.TaskDetails")]
public class TaskDetails
{
    [ForeignKey("UserId")]
    public long UserId { get; set; }
    [ForeignKey("TaskId")]
    public long TaskId { get; set; }
}
