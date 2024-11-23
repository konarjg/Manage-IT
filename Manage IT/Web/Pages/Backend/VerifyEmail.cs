using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class VerifyEmail : PageModel
{
    public IActionResult OnGet(string emailEncrypted)
    {
        if (emailEncrypted == null || emailEncrypted == string.Empty)
        {
            return Redirect("/");
        }

        var email = Security.DecryptText(emailEncrypted);
        UserManager.Instance.VerifyUser(email);

        return Redirect("/");
    }
}
