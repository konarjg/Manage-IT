using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexPage : PageModel
{
    public string Header { get; set; }

    public IActionResult OnGet(string message)
    {
        if (HttpContext.Session.Get<User>("User") != null)
        {
            return Redirect("/ProjectManagement");
        }

        if (message == null || message == string.Empty)
        {
            Header = string.Empty + Header;
            return null;
        }

        Header = message;
        return null;
    }
}