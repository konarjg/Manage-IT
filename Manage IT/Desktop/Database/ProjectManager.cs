using Desktop;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

public class ProjectManager
{
    public static ProjectManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new ProjectManager();
    }

    public bool GetAllProjects(long managerId, out List<Project> projects)
    {
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ManagerId = {managerId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
    }

    public bool GetProject(long projectId, out Project project)
    {
        List<Project> projects;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ProjectId = {projectId}");

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
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.Projects (Name, Description, ManagerId) VALUES ('{data.Name}', '{data.Description}', {data.ManagerId})");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        if (success && App.Instance.UserSettings.SendProjectAlerts)
        {
            var subject = "Manage IT notification: New project has been successfully created on Your account!";
            var body = $"Dear {UserManager.Instance.CurrentSessionUser.Login}, <br/>A project named {data.Name} has been created and can now be managed in Your project panel!<br/>Happy Managing IT!";
            string error;

            EmailService.SendEmail(UserManager.Instance.CurrentSessionUser.Email, subject, body, out error);
        }

        return success;
    }

    public bool UpdateProject(Project data)
    {
        List<Project> projects;
        var query = FormattableStringFactory.Create($"UPDATE dbo.Projects SET ManagerId = {data.ManagerId}, Name = '{data.Name}', Description = '{data.Description}' WHERE ProjectId = {data.ProjectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        if (success && App.Instance.UserSettings.SendProjectAlerts)
        {
            var subject = "Manage IT notification: Project has been edited!";
            var body = $"Dear {UserManager.Instance.CurrentSessionUser.Login}, <br/>A project named {data.Name} has been edited, You can view the changes in the project panel!<br/>Happy Managing IT!";
            string error;

            EmailService.SendEmail(UserManager.Instance.CurrentSessionUser.Email, subject, body, out error);
        }

        return success;
    }

    public bool DeleteProject(long projectId)
    {
        List<Meeting> meetings;
        List<Project> projects;
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.Meetings WHERE ProjectId = {projectId}");
        var query1 = FormattableStringFactory.Create($"DELETE FROM dbo.Projects WHERE ProjectId = {projectId}");

        Project data;
        bool success = GetProject(projectId, out data);
        bool success1 = DatabaseAccess.Instance.ExecuteQuery(query, out meetings);
        bool success2 = DatabaseAccess.Instance.ExecuteQuery(query1, out projects);

        if (success && success1 && success2 && App.Instance.UserSettings.SendProjectAlerts)
        {
            var subject = "Manage IT notification: Project has been deleted!";
            var body = $"Dear {UserManager.Instance.CurrentSessionUser.Login}, <br/>A project named {data.Name} has been deleted, IT should now disappear from Your project panel.";
            string error;

            EmailService.SendEmail(UserManager.Instance.CurrentSessionUser.Email, subject, body, out error);
        }

        return success && success1 && success2;
    }

    private bool ProjectExists(string name)
    {
        List<Project> projects;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE Name = '{name}'");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects)
            && projects != null && projects.Count != 0;

        return success;
    }

    public bool GetProjectMembers(long projectId, out List<User> members)
    {
        members = new();

        List<ProjectMembers> data;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.ProjectMembers WHERE ProjectId = {projectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out data) && data != null && data.Count != 0;

        foreach (var entry in data)
        {
            List<User> result;
            var queryUser = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE UserId = {entry.UserId}");
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
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.ProjectMembers (ProjectId, UserId, InviteAccepted) VALUES ({projectId}, {userId}, 0)");

        return DatabaseAccess.Instance.ExecuteQuery(query, out data);
    }

    public bool RemoveProjectMember(Project project, User user)
    {
        List<ProjectMembers> data;
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.ProjectMembers WHERE ProjectId = {project.ProjectId} AND UserId = {user.UserId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out data);

        if (!success)
        {
            return false;
        }

        var subject = "Manage IT Notification: You have been kicked from a project";
        var body = $"Dear {user.Login},<br/>Unfortunately You have been kicked from a project named {project.Name}. If You don't agree with this decision, contact the manager or an administrator!";
        string error;

        return EmailService.SendEmail(user.Email, subject, body, out error);
    }

    public List<ProjectMembers> GetUnacceptedInvites(Project project)
    {
        List<ProjectMembers> data;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.ProjectMembers WHERE InviteAccepted = 0");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out data);

        if (!success)
        {
            return null;
        }

        return data;
    }
    public bool AcceptInvite(Project project,User user)
    {
    List<ProjectMembers> data;
    var query = FormattableStringFactory.Create($"UPDATE dbo.ProjectMembers SET InviteAccepted = 1 WHERE UserId = {user.UserId} AND ProjectId = {project.ProjectId}");

    bool success = DatabaseAccess.Instance.ExecuteQuery(query, out data);

    return success;
    }
}
