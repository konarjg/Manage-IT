using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web;

public class ForgotPasswordForm : PageModel
{
    public string Error { get; set; }

    public IActionResult OnPost(string credential)
    {
        if (credential == null)
        {
            Error = "You have to specify an email or a username!";
            return null;
        }

        User data = new()
        {
            Email = credential,
            Login = credential
        };

        User user;

        if (!UserManager.Instance.UserExists(data, out user))
        {
            Error = "Account with specified credentials doesn't exist!";
            return null;
        }

        var url = $"manageit.runasp.net/RecoverPassword?userId={user.UserId}";
        var subject = "Manage IT Alert: Password recovery request";
        var body = $"Dear {user.Login},<br/>a password recovery has been requested using Your credentials.<br/>If this was You click the following link.<br/>Otherwise change Your login credentials immediately!<br/><a href='{url}'>Click here to recover Your password!</a>";
        string error;

        bool success = EmailService.SendEmail(user.Email, subject, body, out error);

        if (!success)
        {
            Error = error;
            return null;
        }

        var message = "Password recovery instructions have been sent to Your email!";
        return Redirect($"~/?message={message}");
    }
}