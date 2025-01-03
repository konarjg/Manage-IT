using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("ProjectMembers")]
public class ProjectMembers
{
    [ForeignKey("UserId")]
    public long UserId { get; set; }
    [ForeignKey("ProjectId")]
    public long ProjectId { get; set; }
    public bool InviteAccepted { get; set; }
}
