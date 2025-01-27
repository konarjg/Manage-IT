using Desktop;
using Desktop.Database;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class UserManager
{
    public User CurrentSessionUser { get; set; }

    public static UserManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new UserManager();
    }

    public bool SendPasswordRestorationEmail(User user, out string error)
    {
        User existingUser;
        if (!UserExists(user, out existingUser))
        {
            error = "No account found with this email or login!";
            return false;
        }
        long userId = existingUser.UserId;
        //form will forward hashed
        string hashedPassword = user.Password;
        string subject = "Manage IT Password Restoration";
        string url = $"<a href='http://manageit.runasp.net/RestorePassword?userId={userId}&password={hashedPassword}'>Click to restore!</a>";
        string body = string.Format($"There was a registered attempt to change your password!<br/>If that was you, just click this link below to confirm your password reset:<br/>{url}");
        EmailService.SendEmail(existingUser.Email, subject, body, out error);
        error = string.Empty;
        return true;
    }

    public bool GetUser(long userId, out User user)
    {
        List<User> users;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE UserId = {userId}");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users) && users != null && users.Count != 0;

        if (!success)
        {
            user = null;
            return false;
        }

        user = users[0];
        return true;
    }
    public bool GetAllUsers(out List<User> users)
    {
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users) && users != null && users.Count != 0;

        if (!success)
        {
            users = null;
            return false;
        }

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
        FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.Users (Login,Password,Email,Admin,Verified) VALUES ('{user.Login}', '{user.Password}','{user.Email}', 0, 0)");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);

        if (!success)
        {
            error = "There was an unexpected error! Could not create an account.";
            return false;
        }

        string subject = "Manage IT Account Confirmation";
        string username = user.Login;
        string url = string.Format("http://manageit.runasp.net/VerifyEmail?email={0}", user.Email);
        string body = string.Format("Dear {0},<br>Thank You for choosing Manage IT. <br><a href=\"{1}\">Click here to verify your account</a>", username, url);

        EmailService.SendEmail(user.Email, subject, body, out error);
        error = string.Empty;
        return true;
    }

    public bool GetCurrentUserPermissions(long projectId, out UserPermissions permissions)
    {
        long userId = CurrentSessionUser.UserId;

        List<UserPermissions> records;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.UserPermissions WHERE UserId = {userId} AND ProjectId = {projectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out records);

        if (!success)
        {
            permissions = null;
            return false;
        }

        permissions = records.FirstOrDefault();

        return true;
    }

    public bool GetUserPermissions(long userId, long projectId, out UserPermissions permissions)
    {
        List<UserPermissions> records;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.UserPermissions WHERE UserId = {userId} AND ProjectId = {projectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out records);

        if (!success)
        {
            permissions = null;
            return false;
        }

        permissions = records.FirstOrDefault();

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
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE (Email LIKE '{user.Email}' OR Login LIKE '{user.Login}') AND Password LIKE '{user.Password}' AND Verified = 1");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);
        success = success && users != null && users.Count != 0;

        if (!success)
        {
            if (App.Instance.UserSettings.SendSecurityAlerts)
            {
                string username = existingUser.Login;
                DateTime dateTime = DateTime.Now;
                string successful = "failed";
                string subject = "Alert: New Login Attempt On Your Account!";
                string body = string.Format("Dear {0}, <br>We noticed a {1} login attempt on Your account on {2}. <br>If this was You, no further action is needed. <br>If You did not attempt to log in, please secure Your account immediately by changing Your password.<br>Sincerely<br>Manage IT Team.", username, successful, dateTime.ToString());
                string error;

                EmailService.SendEmail(existingUser.Email, subject, body, out error);
            }
        }
        else
        {
            CurrentSessionUser = users[0];

            if (App.Instance.UserSettings.SendSecurityAlerts)
            {
                string username = CurrentSessionUser.Login;
                DateTime dateTime = DateTime.Now;
                string successful = "successful";
                string subject = "Alert: New Login Attempt On Your Account!";
                string body = string.Format("Dear {0}, <br>We noticed a {1} login attempt on Your account on {2}. <br>If this was You, no further action is needed. <br>If You did not attempt to log in, please secure Your account immediately by changing Your password.<br>Sincerely<br>Manage IT Team.", username, successful, dateTime.ToString());
                string error;

                EmailService.SendEmail(CurrentSessionUser.Email, subject, body, out error);
            }
        }

        return success;
    }

    private bool UserExists(User data)
    {
        List<User> existingUsers;
        FormattableString queryUserExists = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{data.Email}'");
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
        FormattableString queryUserExists = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{data.Email}' OR Login LIKE '{data.Login}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(queryUserExists, out existingUsers) && existingUsers != null && existingUsers.Count != 0;

        if (!success)
        {
            user = null;
            return false;
        }

        user = existingUsers[0];
        return existingUsers.Count != 0;
    }

    public bool UpdateUser(User data)
    {
        List<User> users;
        FormattableString query = FormattableStringFactory.Create($"UPDATE dbo.Users SET Email = '{data.Email}', Login = '{data.Login}', Password = '{data.Password}' WHERE UserId = {data.UserId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out users);
    }

    public bool DisableUser(User user)
    {
        List<User> users;
        FormattableString query = FormattableStringFactory.Create($"UPDATE dbo.Users SET Verified = 0 WHERE Email LIKE '{user.Email}'");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);

        if (!success)
        {
            return false;
        }

        string url = string.Format($"http://manageit.runasp.net/VerifyEmail?email={user.Email}");
        string subject = "Your Manage IT account has been disabled!";
        string body = $"Dear {user.Login}!<br/>Your account has been disabled!<br>If you wish to enable it again click the following link<br/><a href={url}>Click to enable your account</a>";
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
        ChatManager.Instance.DeleteAllConversations(user.UserId);
        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Users WHERE Email LIKE '{user.Email}'");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);

        if (!success)
        {
            return false;
        }

        string subject = "Your Manage IT account has been deleted!";
        string body = $"Dear {user.Login}!<br/>Your account has been deleted!<br>If you wish to restore it, contact an administrator or create a new account!<br/> Thanks for choosing Manage IT!";
        string error;

        EmailService.SendEmail(user.Email, subject, body, out error);

        if (error != "")
        {
            return false;
        }

        return true;
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

        bool success = ProjectManager.Instance.AddProjectMember(project.ProjectId, data.UserId);

        if (!success)
        {
            return false;
        }

        string url = $"http://manageit.runasp.net/AcceptInvite?userId={data.UserId}&projectId={project.ProjectId}";
        string subject = "Manage IT Notification: You have been invited to collaborate on a project";
        string body = $"Dear {user.Login},<br/>You have been invited to collaborate on a project named {project.Name}.<br/>Project description:<br/>{project.Description}<br/>If You wish to accept the invite, click the following link<br/><a href='{url}'>Click here to accept the invite</a><br/>Happy collaborating on IT!";
        string error;


        return EmailService.SendEmail(data.Email, subject, body, out error);
    }

    public void CreatePermissionsForCurrentUser(string name)
    {
        List<UserPermissions> temp;
        FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.UserPermissions (ProjectId, UserId, Editing, InvitingMembers, KickingMembers) SELECT ProjectId, {CurrentSessionUser.UserId}, 1, 1, 1 FROM dbo.Projects WHERE Name LIKE '{name}'");

        DatabaseAccess.Instance.ExecuteQuery(query, out temp);
    }

    public bool DeleteAllPermissions(long projectId)
    {
        List<UserPermissions> permissions;
        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.UserPermissions WHERE ProjectId = {projectId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out permissions);
    }

    public bool UpdateUserPermissions(UserPermissions data)
    {
        List<UserPermissions> permissions;
        FormattableString query = FormattableStringFactory.Create($"UPDATE dbo.UserPermissions SET Editing = {(data.Editing ? 1 : 0)}, InvitingMembers = {(data.InvitingMembers ? 1 : 0)}, KickingMembers = {(data.KickingMembers ? 1 : 0)} WHERE UserId = {data.UserId} AND ProjectId = {data.ProjectId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out permissions);
    }

}
