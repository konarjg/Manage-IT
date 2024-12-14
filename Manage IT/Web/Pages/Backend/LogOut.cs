using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LogOut : PageModel
{
    public IActionResult OnGet()
    {
        if (HttpContext.Session.Get<User>("User") == null)
        {
            return Redirect("~/LoginForm");
        }

        HttpContext.Session.Remove("User");
        return Redirect("~/LoginForm");
    }
}