using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("TaskDetails")]
public class TaskDetails
{
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    [ForeignKey("TaskId")]
    public int TaskId { get; set; }
}
