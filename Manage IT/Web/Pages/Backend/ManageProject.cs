using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Globalization;
using JsonSerializer = System.Text.Json.JsonSerializer;
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
    ManageMember,
    KickMember,
    Delete
}

public class ManageProject : PageModel
{

    public long projectID;
    public string Error { get; set; }
    public Project Project { get; set; }

    public Meeting CurrentlyEditedMeeting = new();
    public User CurrentlyManagedMember = new();
    public UserPermissions currentlyManagedMemberPermissions = new();
    public List<Meeting> Meetings { get; set; }


    public List<User> Members { get; set; }
    public List<bool> InviteStatuses { get; set; } = new(); //ik its not professional but GetInviteStatus for some reason refuses to fucking work properly despite working properly for me in Desktop version and JS not saying anything

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
            HttpContext.Session.Remove("Members");
            HttpContext.Session.Remove("Action");
            Action = ProjectAction.Manage;
        }

        if (HttpContext.Session.Get<Project>("Project") != null)
        {
            Project = HttpContext.Session.Get<Project>("Project");
            Members = HttpContext.Session.Get<List<User>>("Members");
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
            List<User> members;
            bool success = ProjectManager.Instance.GetProject(projectId, out project);

            if (!success || project == null)
            {
                return Redirect("~/ProjectManagement");
            }


            success = ProjectManager.Instance.GetProjectMembers(project.ProjectId, out members);

            if (members == null)
            {
                return Redirect("~/ProjectManagement");
            }

            Project = project;
            Members = members;
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


        if (HttpContext.Session.Get<List<User>>("Members") == null)
        {
            List<User> members;
            //bool success = UserManager.Instance.GetAllUsers(out members);
            bool success = ProjectManager.Instance.GetProjectMembers(Project.ProjectId, out members);

            if (!success || members == null || members.Count == 0)
            {
                Members = new();
            }
            else
            {
                Members = members;
            }

            HttpContext.Session.Set("Members", members);
        }
        else
        {
            Members = HttpContext.Session.Get<List<User>>("Members");
        }

        projectID = Project.ProjectId;
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

        Project data = new Project();
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
        HttpContext.Session.Remove("Members");
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
        bool success = ProjectManager.Instance.GetProjectMembers(HttpContext.Session.Get<Project>("Project").ProjectId, out List<User> members);

        if (!success || members == null || members.Count == 0)
        {
            Members = new();
        }
        else
        {
            Members = members;
        }
        if (members != null)
        {
            foreach (User member in members)
            {
                InviteStatuses.Add(ProjectManager.Instance.GetInviteStatus(HttpContext.Session.Get<Project>("Project").ProjectId, member.UserId));
            }
        }

        HttpContext.Session.Set("Members", members);
        //HttpContext.Session.Set("InviteStatuses", InviteStatuses);
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
    public IActionResult OnPostManageMember(string memberID)
    {
        if (long.TryParse(memberID, out long userIdLong))
        {
            bool found = UserManager.Instance.GetUser(userIdLong, out CurrentlyManagedMember);
            if (!found || CurrentlyManagedMember == null)
            {
                // Handle case where meeting is not found
                return NotFound("Member not found.");
            }
            bool foundPermissions = UserManager.Instance.GetUserPermissions(userIdLong, HttpContext.Session.Get<Project>("Project").ProjectId, out currentlyManagedMemberPermissions);
            if (!foundPermissions || currentlyManagedMemberPermissions == null)
            {
                // Handle case where meeting is not found
                return NotFound("An error has occured.");
            }
            Action = ProjectAction.ManageMember;
            HttpContext.Session.Set("Action", Action);

            TempData["currentlyManagedMember"] = JsonConvert.SerializeObject(CurrentlyManagedMember);
            TempData["currentlyManagedMemberPermissions"] = JsonConvert.SerializeObject(currentlyManagedMemberPermissions);
            return Redirect("~/ManageProject");
        }
        else
        {
            // Handle parsing error
            return BadRequest("Invalid user ID");
        }
    }
    public IActionResult OnPostKickMember(string memberID)
    {
        if (long.TryParse(memberID, out long userIdLong))
        {
            bool found = UserManager.Instance.GetUser(userIdLong, out CurrentlyManagedMember);
            if (!found || CurrentlyManagedMember == null)
            {
                // Handle case where member is not found
                return NotFound("Member not found.");
            }
            
            //here we don't need permissions so just proceed to moving to kicking razorPage
            Action = ProjectAction.KickMember;
            HttpContext.Session.Set("Action", Action);

            TempData["currentlyManagedMember"] = JsonConvert.SerializeObject(CurrentlyManagedMember);
            TempData["currentlyManagedMemberPermissions"] = JsonConvert.SerializeObject(currentlyManagedMemberPermissions);
            return Redirect("~/ManageProject");
        }
        else
        {
            // Handle parsing error
            return BadRequest("Invalid user ID");
        }
    }
    public IActionResult OnPostEditMeeting(string meetingID)
    {
        if (long.TryParse(meetingID, out long meetingIdLong))
        {
            bool found = MeetingManager.Instance.GetMeeting(meetingIdLong, out CurrentlyEditedMeeting);
            if (!found || CurrentlyEditedMeeting == null)
            {
                // Handle case where meeting is not found
                return NotFound("Meeting not found.");
            }

            Action = ProjectAction.EditMeeting;
            HttpContext.Session.Set("Action", Action);

            // Store currentlyEditedMeeting in TempData
            TempData["currentlyEditedMeeting"] = JsonConvert.SerializeObject(CurrentlyEditedMeeting);

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
            bool found = MeetingManager.Instance.GetMeeting(meetingIdLong, out CurrentlyEditedMeeting);
            if (!found || CurrentlyEditedMeeting == null)
            {
                // Handle case where meeting is not found
                return NotFound("Meeting not found.");
            }

            Action = ProjectAction.DeleteMeeting;
            HttpContext.Session.Set("Action", Action);

            // Store currentlyEditedMeeting in TempData
            TempData["currentlyEditedMeeting"] = JsonConvert.SerializeObject(CurrentlyEditedMeeting);

            return Redirect("~/ManageProject");
        }
        else
        {
            // Handle parsing error
            return BadRequest("Invalid meeting ID");
        }
    }

    public JsonResult OnPostKickMemberConfirm(string memberId,string projectId)
    {
        if (CurrentlyManagedMember == null)
        {
            return new JsonResult(new { success = false, message = "No currently managed member" });
        }

        if (!long.TryParse(memberId, out long memberIdLong))
        {
            return new JsonResult(new { success = false, message = "Invalid member ID." });
        }

        if (!long.TryParse(projectId, out long projectIdLong))
        {
            return new JsonResult(new { success = false, message = "Invalid project ID." });
        }

        bool successMember = UserManager.Instance.GetUser(memberIdLong, out User member);

        bool success = ProjectManager.Instance.RemoveProjectMember(HttpContext.Session.Get<Project>("Project"), member);

        if (!success)
        {
            Error = "Could not delete the meeting!";
            return new JsonResult(new { success = false, message = "Could not kick the member." });
        }
        HttpContext.Session.Remove("currentlyManagedMember");
        HttpContext.Session.Remove("Members");
        Action = ProjectAction.Members;
        HttpContext.Session.Set("Action", Action);
        CurrentlyEditedMeeting = null;
        // Return JSON result indicating success
        return new JsonResult(new { success = true });
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
        HttpContext.Session.Remove("Members");
        HttpContext.Session.Remove("Action");
        return Redirect("~/ProjectManagement");
    }

    public JsonResult OnPostInviteUserToProject(string credential)
    {
        if (credential == null || credential == String.Empty)
        {

            Action = ProjectAction.Manage;
            return new(new { success = false });
        }
        User data = new();
        data.Login = credential;
        data.Email = credential;


        bool successInvite = UserManager.Instance.SendProjectInvite(data, HttpContext.Session.Get<Project>("Project"));
        return new(new { success = successInvite });
    }

    public JsonResult OnPostUpdateTaskList(string taskListJson)
    {
        TaskList? taskList = JsonSerializer.Deserialize<TaskList>(taskListJson);
        bool querySuccess = TaskListManager.Instance.UpdateTaskList(taskList);

        HttpContext.Session.Remove("TaskLists");
        return new(new { success = querySuccess });
    }

    public JsonResult OnPostDeleteTaskList(string taskListJson)
    {
        TaskList? taskList = JsonSerializer.Deserialize<TaskList>(taskListJson);
        bool querySuccess = TaskListManager.Instance.DeleteTaskList(taskList.TaskListId);

        HttpContext.Session.Remove("TaskLists");
        return new(new { success = querySuccess });
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
        return new(new { success = true });


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
        if (CurrentlyEditedMeeting == null)
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
        CurrentlyEditedMeeting = null;
        // Return JSON result indicating success
        return new JsonResult(new { success = true });
    }
    public JsonResult OnPostUpdateMemberPermissionsConfirm(string userID, string projectID, string newEditing, string newInviting, string newKicking)
    {
        // Parse the input parameters
        long parsedUserID = long.Parse(userID);
        long parsedProjectID = long.Parse(projectID);
        bool parsedNewEditing = bool.Parse(newEditing);
        bool parsedNewInviting = bool.Parse(newInviting);
        bool parsedNewKicking = bool.Parse(newKicking);

        UserPermissions data = new UserPermissions
        {
            UserId = parsedUserID,
            ProjectId = parsedProjectID,
            Editing = parsedNewEditing,
            InvitingMembers = parsedNewInviting,
            KickingMembers = parsedNewKicking
        };

        bool success = UserManager.Instance.UpdateUserPermissions(data);
        if (!success)
        {
            Error = "Could not update permissions!";
            return new JsonResult(new { success = false, message = "Could not update permissions!" });
        }
        // Return JSON result indicating success
        Action = ProjectAction.Members;
        HttpContext.Session.Set("Action", Action);
        CurrentlyManagedMember = null;
        currentlyManagedMemberPermissions = null;
        return new JsonResult(new { success = true });
    }



}
