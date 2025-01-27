using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class GetSecurityParameters : PageModel
{
    public ContentResult OnGet()
    {
        ContentResult response = new ContentResult();
        response.Content = Security.Parameters;

        return response;
    }
}
