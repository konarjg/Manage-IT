using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.Prefixes")]
public class Prefix
{
    [Key]
    public short PrefixId { get; set; }
    public string Country { get; set; }

    public override string ToString()
    {
        return string.Format("+{0} {1}", PrefixId, Country);
    }
}
