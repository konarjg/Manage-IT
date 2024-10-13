using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

public class ProjectContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
}

[Table("Projects")]
public class Project
{
    [Key]
    public int ProjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [ForeignKey("UserId")]
    public int ManagerId { get; set; }
}
