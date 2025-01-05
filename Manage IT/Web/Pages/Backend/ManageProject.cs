using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public enum ProjectAction
{
    Manage,
    Info,
    Members,
    Update,
    CreateMeeting,
    Delete
}

public class ManageProject : PageModel
{
    public string Error { get; set; }
    public Project Project { get; set; }
    public List<User> Members { get; set; } = new()
    {
        new()
        {
            Email = "konarskikrzysztof1@gmail.com",
            Login = "konis"
        },
        new()
        {
            Email = "272844@student.pwr.edu.pl",
            Login = "konar"
        },
        new()
        {
            Email = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@gmail.com",
            Login = "konis"
        },
        new()
        {
            Email = "272844@student.pwr.edu.pl",
            Login = "konar"
        },
        new()
        {
            Email = "konarskikrzysztof1@gmail.com",
            Login = "konis"
        },
        new()
        {
            Email = "272844@student.pwr.edu.pl",
            Login = "konar"
        },
        new()
        {
            Email = "konarskikrzysztof1@gmail.com",
            Login = "konis"
        },
        new()
        {
            Email = "272844@student.pwr.edu.pl",
            Login = "konar"
        }
    };

    public List<TaskList> TaskLists { get; set; }
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
        
        if (HttpContext.Session.Get<List<TaskList>>("TaskLists") == null)
        {
            List<TaskList> taskLists;
            success = TaskListManager.Instance.GetAllTaskLists(projectId, out taskLists);

            if (!success || taskLists == null || taskLists.Count == 0)
            {
                TaskLists = new();
            }
            else
            {
                TaskLists = taskLists;
            }

            HttpContext.Session.Set("TaskLists", taskLists);
        }
        else
        {
            TaskLists = HttpContext.Session.Get<List<TaskList>>("TaskLists");
        }
        

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

    public IActionResult OnPostConfirmMeeting(string title, string description, DateTime date)
    {
        if (HttpContext.Session.Get<Project>("Project") == null)
        {
            return Redirect("~/ProjectManagement");
        }

        Project = HttpContext.Session.Get<Project>("Project");

        if (title == null || description == null)
        {
            Error = "You have to fill in every field!";
            return null;
        }

        Meeting data = new();
        data.ProjectId = Project.ProjectId;
        data.Title = title;
        data.Description = description;
        data.Date = date;

        bool success = MeetingManager.Instance.CreateMeeting(data);

        if (!success)
        {
            Error = "Could not create the meeting!";
            return null;
        }

        Action = ProjectAction.Manage;
        HttpContext.Session.Set("Action", Action);
        HttpContext.Session.Remove("Meetings");
        return Redirect("~/ManageProject");
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

    public IActionResult OnPostCreateMeeting()
    {
        Action = ProjectAction.CreateMeeting;
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

    public JsonResult OnPostCreateTaskList(string projectId, string name, string description)
    {
        if (name == null || description == null)
        {
            return new(new { success = false });
        }

        TaskList data = new();
        data.ProjectId = long.Parse(projectId);
        data.Name = name;
        data.Description = description;

        bool success = TaskListManager.Instance.CreateTaskList(data);

        if (!success)
        {
            return new(new { success = false });
        }

        HttpContext.Session.Remove("TaskLists");
        return new(new{ success = true });
    }

    public JsonResult OnPostCreateTask(string taskListId, string name, string description, string deadline)
    {
        if (name == null || description == null)
        {
            return new(new { success = false });
        }

        Task data = new();
        data.TaskListId = long.Parse(taskListId);
        data.Name = name;
        data.Description = description;
        data.Deadline = DateTime.Parse(deadline);

        bool success = TaskManager.Instance.CreateTask(data);

        if (!success)
        {
            return new(new { success = false });
        }

        HttpContext.Session.Remove("TaskLists");
        return new(new { success = true });
    }
}
