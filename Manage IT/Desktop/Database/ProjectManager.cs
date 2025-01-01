using Desktop;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
}