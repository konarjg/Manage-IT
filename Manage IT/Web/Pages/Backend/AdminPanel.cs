using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public enum AdminPageTemplate
{
    Users = 0,
    Projects = 1,
    TaskLists = 2,
    Tasks = 3,
    Meetings = 4,
    Conversations = 5,
    Messages = 6
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
    public List<Task> Tasks { get; set; }
    public List<Meeting> Meetings { get; set; }
    public List<Conversation> Conversations { get; set; }
    public List<Message> Messages { get; set; }

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

    public JsonResult OnPostRemoveUser(long userId)
    {
        bool querySuccess = UserManager.Instance.RemoveUser(userId);

        return new(new { success = querySuccess });
    }

    public JsonResult OnPostRemoveProject(long projectId)
    {
        bool querySuccess = ProjectManager.Instance.DeleteProject(projectId);

        return new(new { success = querySuccess });
    }

    public JsonResult OnPostRemoveTaskList(long taskListId)
    {
        bool querySuccess = TaskListManager.Instance.DeleteTaskList(taskListId);

        return new(new { success = querySuccess });
    }

    public JsonResult OnPostRemoveTask(long taskId)
    {
        bool querySuccess = TaskManager.Instance.DeleteTask(taskId);

        return new(new { success = querySuccess });
    }

    public JsonResult OnPostRemoveMeeting(long meetingId)
    {
        bool querySuccess = MeetingManager.Instance.DeleteMeeting(meetingId);

        return new(new { success = querySuccess });
    }

    public JsonResult OnPostRemoveConversation(long conversationId)
    {
        bool querySuccess = ChatManager.Instance.DeleteConversation(conversationId);

        return new(new { success = querySuccess });
    }

    public JsonResult OnPostRemoveMessage(long messageId)
    {
        bool querySuccess = ChatManager.Instance.DeleteMessage(messageId);

        return new(new { success = querySuccess });
    }
}