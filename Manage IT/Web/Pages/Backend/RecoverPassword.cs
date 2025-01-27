using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.RegularExpressions;

public class RecoverPassword : PageModel
{
    public string Error { get; set; }
    private long UserId;
    private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");

    public void OnGet(long userId)
    {
        UserId = userId;
    }

    public IActionResult OnPost(string password, string confirmPassword)
    {
        if (password == null || confirmPassword == null)
        {
            Error = "You have to specify a new password and confirm it!";
            return null;
        }

        if (PasswordValidation.IsMatch(password))
        {
            Error = "Password must be at least 8 characters long, contain at least 1 special character, at least 1 uppercase letter and at least 1 number!";
            return null;
        }

        password = Security.HashText(password, Encoding.ASCII);
        confirmPassword = Security.HashText(confirmPassword, Encoding.ASCII);

        if (password != confirmPassword)
        {
            Error = "Provided passwords aren't identical!";
            return null;
        }

        User data;
        bool success = UserManager.Instance.GetUser(UserId, out data);

        if (!success)
        {
            Error = "There was an unexpected error!";
            return null;
        }

        success = UserManager.Instance.UpdateUser(data);

        if (!success)
        {
            Error = "There was an unexpected error!";
            return null;
        }

        data.Password = password;

        var message = "Password has successfully been changed!";
        return Redirect($"~/?message={message}");
    }
}
