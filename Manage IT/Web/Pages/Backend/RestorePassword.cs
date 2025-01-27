using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;
using Web;

public class RestorePassword : PageModel
{
    public IActionResult OnGet(long userId, string password)
    {
        var message = "";
        bool success = UserManager.Instance.RestorePassword(userId, password);

        if (!success)
        {
            message = "There was an unexpected error!";
            return Redirect($"~/?message={message}");
        }

        User user;
        success = UserManager.Instance.GetUser(userId, out user);


        if (!success)
        {
            message = "There was an unexpected error!";
            return Redirect($"~/?message={message}");
        }

        var subject = "Alert: Your Manage IT password has been changed!";
        var body = $"Dear {user.Login},<br/>Your password has successfully been changed during recovery process. <br/>If this wasn't You, contact the administrator immediately!";
        string error;

        EmailService.SendEmail(user.Email, subject, body, out error);

        message = "Password has been restored!";
        return Redirect($"~/?message={message}");
    }
}