using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

public enum ProjectAction
{
    Manage,
    Info,
    Members,
    Update,
    Delete
}

public class ManageProject : PageModel
{
    public string Error { get; set; }
    public Project Project { get; set; }
    public ProjectAction Action { get; set; }

    public IActionResult OnGet(string id)
    {
        if (HttpContext.Session.Get<User>("User") == null)
        {
            return Redirect("~/LoginForm");
        }

        if (id != null && id != string.Empty)
        {
            HttpContext.Session.Remove("Project");
            HttpContext.Session.Remove("Action");
        }

        if (HttpContext.Session.Get<Project>("Project") != null)
        {
            Project = HttpContext.Session.Get<Project>("Project");
            Action = HttpContext.Session.Get<ProjectAction>("Action");
            return null;
        }

        if (id == null || id == string.Empty)
        {
            return Redirect("~/ProjectManagement");
        }

        long projectId;

        if (!long.TryParse(id, out projectId))
        {
            return Redirect("~/ProjectManagement");
        }

        Project project;
        bool success = ProjectManager.Instance.GetProject(projectId, out project);

        if (!success || project == null)
        {
            return Redirect("~/ProjectManagement");
        }

        Project = project;

        HttpContext.Session.Set("Project", project);
        Action = ProjectAction.Manage;
        HttpContext.Session.Set("Action", Action);
        return null;
    }

    public IActionResult OnPost(string name, string description)
    {
        if (HttpContext.Session.Get<Project>("Project") == null)
        {
            return Redirect("~/ProjectManagement");
        }

        Project = HttpContext.Session.Get<Project>("Project");

        if (name == null)
        {
            name = Project.Name;
        }

        if (description == null)
        {
            description = Project.Description;
        }

        var data = new Project();
        data.ProjectId = Project.ProjectId;
        data.ManagerId = Project.ManagerId;
        data.Name = name;
        data.Description = description;

        bool success = ProjectManager.Instance.UpdateProject(data);
        
        if (!success)
        {
            Error = "Could not edit the project!";
            return null;
        }

        string url = "~/ManageProject?id=" + data.ProjectId;
        HttpContext.Session.Remove("Project");
        HttpContext.Session.Remove("Action");
        return Redirect(url);
    }

    public IActionResult OnPostConfirm(string name)
    {
        if (HttpContext.Session.Get<Project>("Project") == null)
        {
            return Redirect("~/ProjectManagement");
        }

        Project = HttpContext.Session.Get<Project>("Project");

        if (name == null)
        {
            Error = "Confirm the project's name!";
            return null;
        }

        bool success = ProjectManager.Instance.DeleteProject(Project.ProjectId);

        if (!success)
        {
            Error = "Could not delete the project!";
            return null;
        }

        HttpContext.Session.Remove("Project");
        HttpContext.Session.Remove("Action");
        return Redirect("~/ProjectManagement");
    }

    public IActionResult OnPostInfo()
    {
        Action = ProjectAction.Info;
        HttpContext.Session.Set("Action", Action);
        return Redirect("~/ManageProject");
    }

    public IActionResult OnPostMembers()
    {
        Action = ProjectAction.Members;
        HttpContext.Session.Set("Action", Action);
        return Redirect("~/ManageProject");
    }

    public IActionResult OnPostUpdate()
    {
        Action = ProjectAction.Update;
        HttpContext.Session.Set("Action", Action);
        return Redirect("~/ManageProject");
    }

    public IActionResult OnPostDelete()
    {
        Action = ProjectAction.Delete;
        HttpContext.Session.Set("Action", Action);
        return Redirect("~/ManageProject");
    }

    public IActionResult OnPostBack()
    {
        Action = ProjectAction.Manage;
        HttpContext.Session.Set("Action", Action);
        return Redirect("~/ManageProject");
    }

    public IActionResult OnPostManage()
    {
        HttpContext.Session.Remove("Project");
        HttpContext.Session.Remove("Action");
        return Redirect("~/ProjectManagement");
    }
}
