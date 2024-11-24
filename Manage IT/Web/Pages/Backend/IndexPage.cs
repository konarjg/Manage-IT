using EFModeling.EntityProperties.DataAnnotations.Annotations;
using MediaBrowser.Model.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;
using System.Text;

public class IndexPage : PageModel
{
    public string Header { get; set; }

    public void OnGet(string message)
    {
        if (message == null || message == string.Empty)
        {
            Header = string.Empty + Header;
            return;
        }

        Header = message;
    }
}