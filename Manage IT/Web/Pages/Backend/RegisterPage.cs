using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

public class RegisterPage : PageModel
{
    public string Error { get; set; }

    public void OnGet()
    {

    }

    public void OnPost()
    {
        string login = Request.Form["Login"];
        string email = Request.Form["Email"];
        string password = Security.HashText(Request.Form["Password"], Encoding.ASCII);
        string confirmPassword = Security.HashText(Request.Form["ConfirmPassword"], Encoding.ASCII);
        string country = Request.Form["Country"];
        int phoneNumber = int.Parse(Request.Form["PhoneNumber"]);

        if (password != confirmPassword)
        {
            Error = "Provided passwords aren't identical!";
            return;
        }

        var user = new User();
        user.Login = login;
        user.Email = email;
        user.Password = password;
        user.PhoneNumber = phoneNumber;
        user.PrefixId = PrefixManager.Instance.GetPrefixByCountry(country).PrefixId;

        if (UserManager.Instance.RegisterUser(user))
        {
            return;
        }
        
        Error = "Could not create an account!";
    }
}
