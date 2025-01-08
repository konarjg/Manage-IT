using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class AccountManagement : PageModel
{
    public User User { get; set; }

    public IActionResult OnGet()
    {
        User = HttpContext.Session.Get<User>("User");
        return null;
    }
}