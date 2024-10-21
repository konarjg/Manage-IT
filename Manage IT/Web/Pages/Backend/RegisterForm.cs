using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

public class RegisterForm : PageModel
{
    public string Error { get; private set; }

    public void OnGet()
    {
        
    }
}
