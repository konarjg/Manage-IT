using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

internal class PrefixContext : DbContext
{
    public DbSet<Prefix> Prefixes { get; set; }
}

[Table("Prefixes")]
public class Prefix
{
    [Key]
    public int PrefixId { get; set; }
    public string Country { get; set; }
}
