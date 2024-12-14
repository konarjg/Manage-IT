using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.UserPermissions")]
public class UserPermissions
{
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    [ForeignKey("ProjectId")]
    public int ProjectId { get; set; }
    public bool Editing { get; set; }
    public bool InvitingMembers { get; set; }
    public bool KickingMembers { get; set; }
}
