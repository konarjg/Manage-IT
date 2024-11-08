using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;

public class RegisterForm : PageModel
{
    public string Error { get; set; }

    private Regex EmailValidation = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}");
    private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");
    private Regex PhoneNumberValidation = new Regex("^{4,15}");

    public string GetFlag(string country)
    {
        var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));
        var englishRegion = regions.FirstOrDefault(region => region.EnglishName.Contains(country));
        
        if (englishRegion == null)
        {
            return "🏳";
        }

        var countryAbbrev = englishRegion.TwoLetterISORegionName;
        return ConvertToFlag(countryAbbrev);
    }
    public string ConvertToFlag(string countryCode) => string.Concat(countryCode.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));

    public void OnGet()
    {
        Error = "";
    }

    public void OnPost(string login, string email, string password, string confirmPassword, string country, string phoneNumber)
    {
        if (login == null || email == null || password == null 
            || confirmPassword == null 
            || country == null || phoneNumber == null)
        {
            Error = "You have to fill in all required fields!";
            return;
        }

        if (!EmailValidation.IsMatch(email)) 
        {
            Error = "Provided email is incorrect!";
            return;
        }

        if (PasswordValidation.IsMatch(password))
        {
            Error = "Password must be at least 8 characters long, contain at least 1 special character, at least 1 uppercase letter and at least 1 number!";
            return;
        }

        if (!PhoneNumberValidation.IsMatch(phoneNumber))
        {
            Error = "Enter a valid phone number!";
            return;
        }

        password = Security.HashText(password, Encoding.ASCII);
        confirmPassword = Security.HashText(confirmPassword, Encoding.ASCII);
        phoneNumber = Security.EncryptText(phoneNumber);

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

        var prefix = PrefixManager.Instance.GetPrefixByCountry(country);
        
        if (prefix == null)
        {
            user.PrefixId = 0;
        }
        else
        {
            user.PrefixId = prefix.PrefixId;
        }

        if (UserManager.Instance == null)
        {
            Error = "There was an unexpected error! Could not create an account.";
            return;
        }

        string error;

        if (UserManager.Instance.RegisterUser(user, out error))
        {
            //TODO Redirect to project management panel
            Error = Security.DecryptText(phoneNumber);
            return;
        }

        Error = error;
    }
}
