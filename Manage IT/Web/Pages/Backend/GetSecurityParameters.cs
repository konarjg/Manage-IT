using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class GetSecurityParameters : PageModel
{
    public ContentResult OnGet()
    {
        var response = new ContentResult();
        response.Content = Security.Parameters;

        return response;
    }
}
