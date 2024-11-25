using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web;

public class VerifyEmail : PageModel
{
    public string Message { get; set; }

    public IActionResult OnGet(string email)
    {
        if (email == null || email == string.Empty)
        {
            Message = "There was an unexpected error!";
            return Redirect($"~/?message={Message}");
        }

        bool success = UserManager.Instance.VerifyUser(email);

        string error;
        Message = success ? "Your account has been verified!" : string.Empty;
        
        if (success)
        {
            EmailService.SendEmail(email, "Manage IT account verification", "Your account has been verified!", out error);
        }

        return Redirect($"~/?message={Message}");
    }
}
