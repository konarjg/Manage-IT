using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.TaskDetails")]
public class TaskDetails
{
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    [ForeignKey("TaskId")]
    public int TaskId { get; set; }
}
