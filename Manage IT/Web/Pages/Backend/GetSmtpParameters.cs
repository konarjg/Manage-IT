using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web;

public class GetSmtpParameters : PageModel
{
    public ContentResult OnGet()
    {
        ContentResult response = new ContentResult();
        response.Content = EmailService.Parameters;

        return response;
    }
}
