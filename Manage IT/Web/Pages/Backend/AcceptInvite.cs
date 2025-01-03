using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;
using Web;

public class AcceptInvite : PageModel
{
    public IActionResult OnGet(long userId, long projectId)
    {
        var message = "";
        bool success = ProjectManager.Instance.AcceptInvite(projectId, userId);
   
        Project project;
        success = ProjectManager.Instance.GetProject(projectId, out project);

        if (!success)
        {
            message = "There was an unexpected error!";
            return Redirect($"~/?message={message}");
        }

        User user;
        success = UserManager.Instance.GetUser(userId, out user);

        if (!success)
        {
            message = "There was an unexpected error!";
            return Redirect($"~/?message={message}");
        }

        User manager;
        success = UserManager.Instance.GetUser(project.ManagerId, out manager);

        if (!success)
        {
            message = "There was an unexpected error!";
            return Redirect($"~/?message={message}");
        }

        var subject1 = "Manage IT Notification: User accepted an invite to Your project";
        var body1 = $"Dear {manager.Login},<br/>{user.Login} has accepted the invite to Your project named {project.Name}!<br/>Happy Managing IT!";
        var subject2 = "Manage IT Notification: You succesfully joined a project!";
        var body2 = $"Dear {user.Login},<br/>You successfully accepted an invite to project named {project.Name} and can now collaborate on IT!<br/> Check out Your project panel!";
        string error;

        EmailService.SendEmail(manager.Email, subject1, body1, out error);
        EmailService.SendEmail(user.Email, subject2, body2, out error);

        message = "Invite has been accepted!";
        return Redirect($"~/?message={message}");
    }
}