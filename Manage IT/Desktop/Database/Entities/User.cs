using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.Users")]
public class User
{
    [Key]
    public long UserId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public bool Verified { get; set; }
    public bool Admin { get; set; }

    public User() { }

    public User(User user)
    {
        UserId = user.UserId;
        Login = user.Login;
        Password = user.Password;
        Email = user.Email;
        Verified = user.Verified;
        Admin = user.Admin;
    }
}
