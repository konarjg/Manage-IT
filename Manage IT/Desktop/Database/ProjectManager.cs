using Desktop;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class ProjectManager
{
    public static ProjectManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new ProjectManager();
    }

    public List<ProjectMembers> GetUnacceptedInvites(Project project)
    {
        List<ProjectMembers> data;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.ProjectMembers WHERE InviteAccepted = 0");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out data);

        if (!success)
        {
            return null;
        }

        return data;
    }

    public bool AcceptInvite(Project project, User user)
    {
        List<ProjectMembers> data;
        System.FormattableString query = FormattableStringFactory.Create($"UPDATE dbo.ProjectMembers SET InviteAccepted = 1 WHERE UserId = {user.UserId} AND ProjectId = {project.ProjectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out data);

        return success;
    }

    public Project GetProjectByName(string name)
    {
        List<Project> results;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE Name = {name}");

        if (!DatabaseAccess.Instance.ExecuteQuery(query, out results) || results == null || results.Count == 0)
        {
            return null;
        }

        return results.FirstOrDefault();
    }
    public Project GetProjectById(long id)
    {
        List<Project> results;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ProjectId = {id}");

        if (!DatabaseAccess.Instance.ExecuteQuery(query, out results) || results == null || results.Count == 0)
        {
            return null;
        }

        return results.FirstOrDefault();
    }
    public bool GetAllProjects(out List<Project> projects)
    {
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects) && projects != null && projects.Count != 0;

        if (!success)
        {
            projects = null;
            return false;
        }

        return true;
    }

    public void DeleteOwnedProjects(long managerId)
    {
        List<Project> results;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ManagerId = {managerId}");

        DatabaseAccess.Instance.ExecuteQuery(query, out results);

        foreach (Project project in results)
        {
            DeleteProject(project.ProjectId);
        }
    }

    public bool GetAllProjects(long userId, out List<Project> projects)
    {
        projects = new();
        List<Project> owned = null;
        List<Project> shared = null;

        bool success = GetOwnedProjects(userId, out owned) && GetSharedProjects(userId, out shared);

        if (!success)
        {
            return false;
        }

        projects.AddRange(owned);
        projects.AddRange(shared);

        return true;
    }

    public bool GetOwnedProjects(long managerId, out List<Project> projects)
    {
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ManagerId = {managerId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
    }

    public bool GetSharedProjects(long userId, out List<Project> projects)
    {
        System.FormattableString query = FormattableStringFactory.Create($"SELECT p.* FROM dbo.Projects p JOIN dbo.ProjectMembers pm ON p.ProjectId = pm.ProjectId WHERE pm.UserId = {userId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
    }


    public bool GetProject(long projectId, out Project project)
    {
        List<Project> projects;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ProjectId = {projectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        if (!success || projects == null || projects.Count == 0)
        {
            project = null;
            return false;
        }

        project = projects[0];
        return true;
    }

    public bool CreateProject(Project data)
    {
        if (ProjectExists(data.Name))
        {
            return false;
        }

        List<Project> projects;
        System.FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.Projects (Name, Description, ManagerId) VALUES ('{data.Name}', '{data.Description}', {data.ManagerId})");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        if (success)
        {
            UserManager.Instance.CreatePermissionsForCurrentUser(data.Name);
        }

        if (success && App.Instance.UserSettings.SendProjectAlerts)
        {
            string subject = "Manage IT notification: New project has been successfully created on Your account!";
            string body = $"Dear {UserManager.Instance.CurrentSessionUser.Login}, <br/>A project named {data.Name} has been created and can now be managed in Your project panel!<br/>Happy Managing IT!";
            string error;

            EmailService.SendEmail(UserManager.Instance.CurrentSessionUser.Email, subject, body, out error);
        }

        return success;
    }

    public bool UpdateProject(Project data)
    {
        List<Project> projects;
        System.FormattableString query = FormattableStringFactory.Create($"UPDATE dbo.Projects SET ManagerId = {data.ManagerId}, Name = '{data.Name}', Description = '{data.Description}' WHERE ProjectId = {data.ProjectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        if (success && App.Instance.UserSettings.SendProjectAlerts)
        {
            string subject = "Manage IT notification: Project has been edited!";
            string body = $"Dear {UserManager.Instance.CurrentSessionUser.Login}, <br/>A project named {data.Name} has been edited, You can view the changes in the project panel!<br/>Happy Managing IT!";
            string error;

            EmailService.SendEmail(UserManager.Instance.CurrentSessionUser.Email, subject, body, out error);
        }

        return success;
    }

    public bool DeleteProject(long projectId)
    {
        List<Meeting> meetings;
        List<Project> projects;


        MeetingManager.Instance.GetMeetingsRelatedToProject(projectId, out meetings);
        foreach (Meeting meeting in meetings)
        {
            MeetingManager.Instance.DeleteMeeting(meeting.MeetingId);
        }
        TaskListManager.Instance.DeleteAllTaskLists(projectId);
        UserManager.Instance.DeleteAllPermissions(projectId);
        DeleteAllMembers(projectId);

        System.FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Projects WHERE ProjectId = {projectId}");

        Project data;
        bool success = GetProject(projectId, out data);
        bool success1 = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        if (success && success1 && App.Instance.UserSettings.SendProjectAlerts)
        {
            string subject = "Manage IT notification: Project has been deleted!";
            string body = $"Dear {UserManager.Instance.CurrentSessionUser.Login}, <br/>A project named {data.Name} has been deleted, IT should now disappear from Your project panel.";
            string error;

            EmailService.SendEmail(UserManager.Instance.CurrentSessionUser.Email, subject, body, out error);
        }

        return success && success1;
    }

    private bool ProjectExists(string name)
    {
        List<Project> projects;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE Name = '{name}'");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects)
            && projects != null && projects.Count != 0;

        return success;
    }

    public bool GetProjectMembers(long projectId, out List<User> members)
    {
        members = new();

        List<ProjectMembers> data;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.ProjectMembers WHERE ProjectId = {projectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out data) && data != null && data.Count != 0;

        foreach (ProjectMembers entry in data)
        {
            List<User> result;
            System.FormattableString queryUser = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE UserId = {entry.UserId}");
            bool successUser = DatabaseAccess.Instance.ExecuteQuery(queryUser, out result) && result != null && result.Count != 0;

            if (successUser && result.FirstOrDefault() != null)
            {
                members.Add(result.FirstOrDefault());
            }
        }

        return success;
    }

    public bool AddProjectMember(long projectId, long userId)
    {
        List<ProjectMembers> data;
        List<UserPermissions> temp;
        System.FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.ProjectMembers (ProjectId, UserId, InviteAccepted) VALUES ({projectId}, {userId}, 0)");
        System.FormattableString query1 = FormattableStringFactory.Create($"INSERT INTO dbo.UserPermissions (ProjectId, UserId, Editing, InvitingMembers, KickingMembers) VALUES ({projectId}, {userId}, 0, 0, 0)");

        return DatabaseAccess.Instance.ExecuteQuery(query, out data) && DatabaseAccess.Instance.ExecuteQuery(query1, out temp);
    }

    public bool RemoveProjectMember(Project project, User user)
    {
        List<ProjectMembers> data;
        List<UserPermissions> temp;
        System.FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.UserPermissions WHERE ProjectId = {project.ProjectId} AND UserId = {user.UserId}");
        System.FormattableString query1 = FormattableStringFactory.Create($"DELETE FROM dbo.ProjectMembers WHERE ProjectId = {project.ProjectId} AND UserId = {user.UserId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out temp) && DatabaseAccess.Instance.ExecuteQuery(query1, out data);

        if (!success)
        {
            return false;
        }

        string subject = "Manage IT Notification: You have been kicked from a project";
        string body = $"Dear {user.Login},<br/>Unfortunately You have been kicked from a project named {project.Name}. If You don't agree with this decision, contact the manager or an administrator!";
        string error;

        return EmailService.SendEmail(user.Email, subject, body, out error);
    }

    private void DeleteAllMembers(long projectId)
    {
        List<ProjectMembers> data;
        System.FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.ProjectMembers WHERE ProjectId = {projectId}");

        DatabaseAccess.Instance.ExecuteQuery(query, out data);
    }
}
