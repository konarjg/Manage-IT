using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public enum ProjectAction
{
    Manage,
    Info,
    Members,
    Update,
    Meetings,
    EditMeeting,
    DeleteMeeting,
    Delete
}

public class ManageProject : PageModel
{

    public long projectID;
    public string Error { get; set; }
    public Project Project { get; set; }

    public Meeting currentlyEditedMeeting = new();
    public List<Meeting> Meetings { get; set; }

    
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
            Action = ProjectAction.Manage;
        }

        if (HttpContext.Session.Get<Project>("Project") != null)
        {
            Project = HttpContext.Session.Get<Project>("Project");
            Action = HttpContext.Session.Get<ProjectAction>("Action");
        }
        else
        {
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
        }
        
        if (HttpContext.Session.Get<List<TaskList>>("TaskLists") == null)
        {
            List<TaskList> taskLists;
            bool success = TaskListManager.Instance.GetAllTaskLists(Project.ProjectId, out taskLists);

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



        if (HttpContext.Session.Get<List<Meeting>>("Meetings") == null)
        {
            List<Meeting> meetings;
            bool success = MeetingManager.Instance.GetMeetingsRelatedToProject(Project.ProjectId, out meetings);

            if (!success || meetings == null || meetings.Count == 0)
            {
                Meetings = new();
            }
            else
            {
                Meetings = meetings;
            }

            HttpContext.Session.Set("Meetings", meetings);
        }
        else
        {
            Meetings = HttpContext.Session.Get<List<Meeting>>("Meetings");
        }


        HttpContext.Session.Set("Project", Project);
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

        projectID = Project.ProjectId;

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

    public IActionResult OnPostMeetings()
    {
        Action = ProjectAction.Meetings;
        HttpContext.Session.Set("Action", Action);
        return Redirect("~/ManageProject");
    }
    public IActionResult OnPostEditMeeting(string meetingID)
    {
        if (long.TryParse(meetingID, out long meetingIdLong))
        {
            bool found = MeetingManager.Instance.GetMeeting(meetingIdLong, out currentlyEditedMeeting);
            if (!found || currentlyEditedMeeting == null)
            {
                // Handle case where meeting is not found
                return NotFound("Meeting not found.");
            }

            Action = ProjectAction.EditMeeting;
            HttpContext.Session.Set("Action", Action);

            // Store currentlyEditedMeeting in TempData
            TempData["currentlyEditedMeeting"] = JsonConvert.SerializeObject(currentlyEditedMeeting);

            return Redirect("~/ManageProject");
        }
        else
        {
            // Handle parsing error
            return BadRequest("Invalid meeting ID");
        }
    }



    public IActionResult OnPostDeleteMeeting(string meetingID)
    {
        if (long.TryParse(meetingID, out long meetingIdLong))
        {
            bool found = MeetingManager.Instance.GetMeeting(meetingIdLong, out currentlyEditedMeeting);
            if (!found || currentlyEditedMeeting == null)
            {
                // Handle case where meeting is not found
                return NotFound("Meeting not found.");
            }

            Action = ProjectAction.DeleteMeeting;
            HttpContext.Session.Set("Action", Action);

            // Store currentlyEditedMeeting in TempData
            TempData["currentlyEditedMeeting"] = JsonConvert.SerializeObject(currentlyEditedMeeting);

            return Redirect("~/ManageProject");
        }
        else
        {
            // Handle parsing error
            return BadRequest("Invalid meeting ID");
        }
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
    public JsonResult OnPostCreateMeeting(string projectId, string title, string description, string date)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
        {
            return new JsonResult(new { success = false });
        }

        Meeting data = new Meeting
        {
            ProjectId = long.Parse(projectId),
            Title = title,
            Description = description
        };

        // Log the incoming date string for debugging
        Console.WriteLine($"Received date string: {date}");

        // Use TryParseExact with expected format
        bool isValidDate = DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.None,
                                                  out DateTime parsedDate);

        if (!isValidDate)
        {
            // Log the date parsing issue
            Console.WriteLine("Invalid date format. Expected format: yyyy-MM-ddTHH:mm");
            return new JsonResult(new { success = false, message = "Invalid date format" });
        }

        // Log the parsed date
        Console.WriteLine($"Parsed Date: {parsedDate}");

        data.Date = parsedDate;

        bool success = MeetingManager.Instance.CreateMeeting(data);

        if (!success)
        {
            return new JsonResult(new { success = false });
        }

        HttpContext.Session.Remove("Meetings");
        Action = ProjectAction.Meetings;
        HttpContext.Session.Set("Action", Action);
        return new JsonResult(new { success = true });
    }



    public JsonResult OnPostUpdateMeeting(string meetingID, string projectID, string title, string description, string date)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
        {
            return new JsonResult(new { success = false });
        }

        Meeting data = new Meeting
        {
            MeetingId = long.Parse(meetingID),
            ProjectId = long.Parse(projectID),
            Title = title,
            Description = description
        };

        bool isValidDate = DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.None,
                                                  out DateTime parsedDate);

        if (!isValidDate)
        {
            return new JsonResult(new { success = false, message = "Invalid date format" });
        }

        data.Date = parsedDate;

        bool success = MeetingManager.Instance.UpdateMeeting(data);

        if (!success)
        {
            return new JsonResult(new { success = false });
        }

        HttpContext.Session.Remove("Meetings");
        Action = ProjectAction.Meetings;
        HttpContext.Session.Set("Action", Action);
        return new JsonResult(new { success = true });
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
    public JsonResult OnPostDeleteConfirmMeeting(string meetingId)
    {
        if (currentlyEditedMeeting == null)
        {
            return new JsonResult(new { success = false, message = "No currently edited meeting." });
        }

        if (!long.TryParse(meetingId, out long meetingIdLong))
        {
            return new JsonResult(new { success = false, message = "Invalid meeting ID." });
        }

        bool success = MeetingManager.Instance.DeleteMeeting(meetingIdLong);

        if (!success)
        {
            Error = "Could not delete the meeting!";
            return new JsonResult(new { success = false, message = "Could not delete the meeting." });
        }

        HttpContext.Session.Remove("Meetings");
        Action = ProjectAction.Meetings;
        HttpContext.Session.Set("Action", Action);

        // Return JSON result indicating success
        return new JsonResult(new { success = true });
    }


}
