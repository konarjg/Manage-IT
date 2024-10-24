using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;

public class IndexPage : PageModel
{
    public string Message { get; set; }

    public void OnGet()
    {
        
    }
}