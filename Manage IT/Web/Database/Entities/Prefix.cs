using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("Prefixes")]
public class Prefix
{
    [Key]
    public int PrefixId { get; set; }
    public string Country { get; set; }
}
