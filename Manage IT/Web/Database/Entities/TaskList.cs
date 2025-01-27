using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("TaskLists")]
public class TaskList
{
    [Key]
    public long TaskListId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [ForeignKey("ProjectId")]
    public long ProjectId { get; set; }
}
