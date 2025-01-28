using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.TaskDetails")]
public class TaskDetails
{
    [ForeignKey("UserId")]
    public long UserId { get; set; }
    [ForeignKey("TaskId")]
    public long TaskId { get; set; }
}
