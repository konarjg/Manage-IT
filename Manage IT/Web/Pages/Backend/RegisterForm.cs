using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.RegularExpressions;

public class RegisterForm : PageModel
{
    public string Error { get; set; }

    private Regex EmailValidation = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}");
    private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");

    public IActionResult OnGet()
    {
        if (HttpContext.Session.Get<User>("User") != null)
        {
            return Redirect("~/ProjectManagement");
        }

        Error = "";
        return null;
    }

    public IActionResult OnPost(string login, string email, string password, string confirmPassword)
    {
        if (login == null || email == null || password == null
            || confirmPassword == null)
        {
            Error = "You have to fill in all required fields!";
            return null;
        }

        if (!EmailValidation.IsMatch(email))
        {
            Error = "Provided email is incorrect!";
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

        User user = new User();
        user.Login = login;
        user.Email = email;
        user.Password = password;

        if (UserManager.Instance == null)
        {
            Error = "There was an unexpected error! Could not create an account.";
            return null;
        }

        string error;

        if (!UserManager.Instance.RegisterUser(user, out error))
        {
            Error = error;
            return null;
        }

        var message = "Account has successfully been created!";
        return Redirect($"~/?message={message}");
    }
}
