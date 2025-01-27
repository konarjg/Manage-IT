using Web;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Xml.Linq;

public class UserManager
{
    public static User CurrentSessionUser { get; private set; }
    public static UserManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new UserManager();
    }

    public bool GetAllUsers(out List<User> users)
    {
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users");

        return DatabaseAccess.Instance.ExecuteQuery(query, out users);
    }

    public bool UpdateUser(User data)
    {
        List<User> users;
        var query = FormattableStringFactory.Create($"UPDATE dbo.Users SET Email = '{data.Email}', Login = '{data.Login}', Password = '{data.Password}' WHERE UserId = {data.UserId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out users);
    }

    public bool RemoveUser(long userId)
    {
        List<User> users;
        ProjectManager.Instance.DeleteOwnedProjects(userId);
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.Users WHERE UserId = {userId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out users);
    }

    public bool DeleteAllPermissions(long projectId)
    {
        List<UserPermissions> permissions;
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.UserPermissions WHERE ProjectId = {projectId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out permissions);
    }

    public bool DisableUser(User user)
    {
        List<User> users;
        var query = FormattableStringFactory.Create($"UPDATE dbo.Users SET Verified = 0 WHERE Email LIKE '{user.Email}'");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);

        if (!success)
        {
            return false;
        }

        var url = string.Format($"http://manageit.runasp.net/VerifyEmail?email={user.Email}");
        var subject = "Your Manage IT account has been disabled!";
        var body = $"Dear {user.Login}!<br/>Your account has been disabled!<br>If you wish to enable it again click the following link<br/><a href={url}>Click to enable your account</a>";
        string error;

        EmailService.SendEmail(user.Email, subject, body, out error);

        if (error != "")
        {
            return false;
        }

        return true;
    }

    public bool DeleteUser(User user)
    {
        List<User> users;
        ProjectManager.Instance.DeleteOwnedProjects(user.UserId);
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.Users WHERE Email LIKE '{user.Email}'");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);

        if (!success)
        {
            return false;
        }

        var subject = "Your Manage IT account has been deleted!";
        var body = $"Dear {user.Login}!<br/>Your account has been deleted!<br>If you wish to restore it, contact an administrator or create a new account!<br/> Thanks for choosing Manage IT!";
        string error;

        EmailService.SendEmail(user.Email, subject, body, out error);

        if (error != "")
        {
            return false;
        }

        return true;
    }

    public void ResetUser()
    {
        CurrentSessionUser = null;
    }

    public bool RestorePassword(long userId, string password)
    {
        List<User> users;
        var query = FormattableStringFactory.Create($"UPDATE dbo.Users SET Password = '{password}' WHERE UserId = {userId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out users);
    }

    public bool GetUser(long userId, out User user)
    {
        List<User> users;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE UserId = {userId}");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users) && users != null && users.Count != 0;

        if (!success)
        {
            user = null;
            return false;
        }

        user = users[0];
        return true;
    }

    public bool GetUserPermissions(long userId, long projectId, out UserPermissions permissions)
    {
        List<UserPermissions> records;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.UserPermissions WHERE UserId = {userId} AND ProjectId = {projectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out records);

        if (!success)
        {
            permissions = null;
            return false;
        }

        permissions = records.FirstOrDefault();

        return true;
    }
    public bool RegisterUser(User user, out string error)
    {
        if (UserExists(user))
        {
            error = "Account with this email already exists!";
            return false;
        }

        List<User> users;
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.Users (Login,Password,Email,Admin,Verified) VALUES ('{user.Login}', '{user.Password}','{user.Email}', 0, 0)");

        var success = DatabaseAccess.Instance.ExecuteQuery(query, out users);

        if (!success)
        {
            error = "There was an unexpected error! Could not create an account.";
            return false;
        }

        var subject = "Manage IT Account Confirmation";
        var username = user.Login;
        var url = string.Format("http://manageit.runasp.net/VerifyEmail?email={0}", user.Email);
        var body = string.Format("Dear {0},<br>Thank You for choosing Manage IT. <br><a href=\"{1}\">Click here to verify your account</a>", username, url);

        EmailService.SendEmail(user.Email, subject, body, out error);
        error = string.Empty;
        return true;
    }

    public bool LoginUser(User user)
    {
        User existingUser;

        if (!UserExists(user, out existingUser))
        {
            return false;
        }

        List<User> users;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE (Email LIKE '{user.Email}' OR Login LIKE '{user.Login}') AND Password LIKE '{user.Password}' AND Verified = 1");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);
        success = success && users != null && users.Count != 0;

        if (!success)
        {
            var username = existingUser.Login;
            var dateTime = DateTime.Now;
            var successful = "failed";
            var subject = "Alert: New Login Attempt On Your Account!";
            var body = string.Format("Dear {0}, <br>We noticed a {1} login attempt on Your account on {2}. <br>If this was You, no further action is needed. <br>If You did not attempt to log in, please secure Your account immediately by changing Your password.<br>Sincerely<br>Manage IT Team.", username, successful, dateTime.ToString());
            string error;

            EmailService.SendEmail(existingUser.Email, subject, body, out error);
        }
        else
        {
            CurrentSessionUser = users[0];
            var username = CurrentSessionUser.Login;
            var dateTime = DateTime.Now;
            var successful = "successful";
            var subject = "Alert: New Login Attempt On Your Account!";
            var body = string.Format("Dear {0}, <br>We noticed a {1} login attempt on Your account on {2}. <br>If this was You, no further action is needed. <br>If You did not attempt to log in, please secure Your account immediately by changing Your password.<br>Sincerely<br>Manage IT Team.", username, successful, dateTime.ToString());
            string error;

            EmailService.SendEmail(CurrentSessionUser.Email, subject, body, out error);
        }

        return success;
    }

    public bool VerifyUser(string email)
    {
        List<User> users;
        var checkVerificationQuery = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{email}' AND Verified = 1");
        bool verified = DatabaseAccess.Instance.ExecuteQuery(checkVerificationQuery, out users) && users != null && users.Count != 0;

        if (verified)
        {
            return false;
        }

        var query = FormattableStringFactory.Create($"UPDATE dbo.Users SET Verified = 1 WHERE Email LIKE '{email}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);
        return success;
    }

    private bool UserExists(User user)
    {
        List<User> existingUsers;
        var queryUserExists = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{user.Email}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(queryUserExists, out existingUsers);

        if (existingUsers == null || !success)
        {
            return false;
        }

        return existingUsers.Count != 0;
    }

    public bool UserExists(User data, out User user)
    {
        List<User> existingUsers;
        var queryUserExists = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{data.Email}' OR Login LIKE '{data.Login}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(queryUserExists, out existingUsers);

        if (existingUsers == null || !success || existingUsers.Count == 0)
        {
            user = null;
            return false;
        }

        user = existingUsers[0];
        return true;
    }
    private bool UserExistsAllowID(User data, out User user)
    {
        List<User> existingUsers;
        var queryUserExists = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{data.Email}' OR Login LIKE '{data.Login}' OR UserID Like '{data.UserId}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(queryUserExists, out existingUsers);

        if (existingUsers == null || !success || existingUsers.Count == 0)
        {
            user = null;
            return false;
        }

        user = existingUsers[0];
        return true;
    }
    public bool UpdateUserPermissions(UserPermissions data)
    {
        List<UserPermissions> permissions;
        var query = FormattableStringFactory.Create($"UPDATE dbo.UserPermissions SET Editing = {(data.Editing ? 1 : 0)}, InvitingMembers = {(data.InvitingMembers ? 1 : 0)}, KickingMembers = {(data.KickingMembers ? 1 : 0)} WHERE UserId = {data.UserId} AND ProjectId = {data.ProjectId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out permissions);
    }
    
    public bool SendProjectInvite(User user, Project project)
{
    User data;

    if (!UserExists(user, out data))
    {
        return false;
    }

    if (data.UserId == project.ManagerId)
    {
        return false;
    }

    // Check if the user is already a member of the project
    if (ProjectManager.Instance.IsProjectMember(project.ProjectId, data.UserId))
    {
        // Check if the user has already accepted the invite
        if (ProjectManager.Instance.GetInviteStatus(project.ProjectId, data.UserId))
        {
            return false; // User is already a member and has accepted the invite
        }
        else
        {
            // Resend the invite if the user hasn't accepted the invite
            return SendInviteEmail(user, project, data);
        }
    }

    bool success = ProjectManager.Instance.AddProjectMember(project.ProjectId, data.UserId);

    if (!success)
    {
        return false;
    }

    return SendInviteEmail(user, project, data);
}


private bool SendInviteEmail(User user, Project project, User data)
{
    var url = $"http://manageit.runasp.net/AcceptInvite?userId={data.UserId}&projectId={project.ProjectId}";
    var subject = "Manage IT Notification: You have been invited to collaborate on a project";
    var body = $"Dear {user.Login},<br/>You have been invited to collaborate on a project named {project.Name}.<br/>Project description:<br/>{project.Description}<br/>If You wish to accept the invite, click the following link<br/><a href='{url}'>Click here to accept the invite</a><br/>Happy collaborating on IT!";
    string error;

    return EmailService.SendEmail(data.Email, subject, body, out error);
}

}
