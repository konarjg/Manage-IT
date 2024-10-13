﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

public class ProjectMembersContext : DbContext
{
    public DbSet<ProjectMembers> ProjectMembers { get; set; }
}

[Table("ProjectMembers")]
public class ProjectMembers
{
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    [ForeignKey("ProjectId")]
    public int ProjectId { get; set; }
}