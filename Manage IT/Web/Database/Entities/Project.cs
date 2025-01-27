using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.Projects")]
public class Project
{
    [Key]
    public long ProjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [ForeignKey("UserId")]
    public long ManagerId { get; set; }
}
