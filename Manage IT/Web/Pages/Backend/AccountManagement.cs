using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.RegularExpressions;

public class AccountManagement : PageModel
{
    public User User { get; set; }
    private Regex EmailValidation = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}");
    private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");

    public IActionResult OnGet()
    {
        User = HttpContext.Session.Get<User>("User");
        return null;
    }

    public JsonResult OnPostDelete()
    {
        User? data = HttpContext.Session.Get<User>("User");

        bool success = UserManager.Instance.DeleteUser(data);

        HttpContext.Session.Remove("User");

        if (!success)
        {
            return new(new { success = false, redirect = Url.Page("/AccountManagement") });
        }

        return new(new { success = true, redirect = Url.Page("/LoginForm") });
    }

    public JsonResult OnPostDisable()
    {
        User? data = HttpContext.Session.Get<User>("User");

        bool success = UserManager.Instance.DisableUser(data);

        HttpContext.Session.Remove("User");

        if (!success)
        {
            return new(new { success = false, redirect = Url.Page("/AccountManagement") });
        }

        return new(new { success = true, redirect = Url.Page("/LoginForm") });
    }

    public JsonResult OnPostEdit(string login, string email, string password, string confirmPassword)
    {
        User? data = HttpContext.Session.Get<User>("User");

        if (login == null || email == null || password == null || confirmPassword == null)
        {
            login = data.Login;
            email = data.Email;
            password = data.Password;
        }
        else if (login == "" || email == "" || password == "" || confirmPassword == "")
        {
            login = data.Login;
            email = data.Email;
            password = data.Password;
        }
        else
        {
            if (password != confirmPassword)
            {
                return new(new { success = false, message = "Passwords need to match!" });
            }

            if (!EmailValidation.IsMatch(email))
            {
                return new(new { success = false, message = "Provided email is invalid!" });
            }

            if (PasswordValidation.IsMatch(password))
            {
                return new(new { success = false, message = "Password has to contain at least 1 uppercase letter, 1 lowercase letter, 1 number and 1 special symbol!" });
            }

            password = Security.HashText(password, Encoding.UTF8);
        }

        data.Login = login;
        data.Email = email;
        data.Password = password;

        bool success = UserManager.Instance.UpdateUser(data);

        if (!success)
        {
            return new(new { success = false, message = "There was an unexpected error!" });
        }

        return new(new { success = true, message = "Account was edited" });
    }
}