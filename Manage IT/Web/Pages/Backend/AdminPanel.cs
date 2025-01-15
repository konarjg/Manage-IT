using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public enum AdminPageTemplate
{
    Users = 0,
    UserPermissions = 1,
    Projects = 2,
    ProjectMembers = 3,
    TaskLists = 4,
    Tasks = 5,
    TaskDetails = 6,
    Meetings = 7,
    Conversations = 8,
    Messages = 9
}

public static class AdminPageTemplateExtensions
{
    public static string ToHeader(this AdminPageTemplate? template)
    {
        if (template == null)
        {
            return "";
        }

        string result = string.Concat(template.ToString().Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));

        return result;
    }
}

public class AdminPanel : PageModel
{
    public AdminPageTemplate? PageTemplate { get; set; } = AdminPageTemplate.Users;

    public List<User> Users { get; set; }
    public List<Project> Projects { get; set; }
    public List<TaskList> TaskLists { get; set; }

    public void OnGet()
    {
        if (HttpContext.Session.Get<AdminPageTemplate?>("AdminPageTemplate") == null)
        {
            return;
        }

        PageTemplate = HttpContext.Session.Get<AdminPageTemplate?>("AdminPageTemplate");
    }

    public JsonResult OnPostSwitchPageTemplate(int pageId)
    {
        HttpContext.Session.Set("AdminPageTemplate", (AdminPageTemplate?)pageId);
        return new(new { success = true });
    }
}