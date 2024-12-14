using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ProjectManagement : PageModel
{
    public List<Project> Projects { get; set; }

    public bool CreatingProject { get; set; }
    public string Error { get; set; }

    public IActionResult OnGet(string creating)
    {
        if (creating != null && creating != string.Empty)
        {
            CreatingProject = true;
            Error = HttpContext.Session.GetString("Error");
            return null;
        }

        if (HttpContext.Session.Get<User>("User") == null)
        {
            var user = UserManager.CurrentSessionUser;

            if (user == null)
            {
                return Redirect("~/LoginForm");
            }

            HttpContext.Session.Set("User", user);
            UserManager.Instance.ResetUser();
        }

        List<Project> projects;
        var success = ProjectManager.Instance.GetAllProjects(HttpContext.Session.Get<User>("User").UserId, out projects);

        if (!success || projects == null || projects.Count == 0)
        {
            Projects = new List<Project>();
            return null;
        }

        Projects = projects;

        CreatingProject = false;
        return null;
    }

    public IActionResult OnPost(string name, string description)
    {
        if (HttpContext.Session.Get<User>("User") == null)
        {
            return Redirect("~/LoginForm");
        }

        if (name == null || description == null)
        {
            Error = "You have to fill in every field!";
            HttpContext.Session.SetString("Error", Error);
            return null;
        }

        long managerId = HttpContext.Session.Get<User>("User").UserId;
        Project data = new();
        data.ManagerId = managerId;
        data.Name = name;
        data.Description = description;

        bool success = ProjectManager.Instance.CreateProject(data);

        if (!success)
        {
            Error = "Could not create the project!";
            HttpContext.Session.SetString("Error", Error);
            return null;
        }

        Error = string.Empty;
        return Redirect("~/ProjectManagement");
    }
}
