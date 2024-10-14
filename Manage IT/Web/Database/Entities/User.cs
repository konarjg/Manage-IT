using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    [ForeignKey("PrefixId")]
    public int PrefixId { get; set; }
    public int PhoneNumber { get; set; }
}
