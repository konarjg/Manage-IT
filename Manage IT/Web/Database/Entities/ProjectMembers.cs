using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("ProjectMembers")]
public class ProjectMembers
{
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    [ForeignKey("ProjectId")]
    public int ProjectId { get; set; }
    public bool InviteAccepted { get; set; }
}
