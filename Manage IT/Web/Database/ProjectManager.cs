using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Runtime.CompilerServices;
using Web;

public class ProjectManager
{
    public static ProjectManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new ProjectManager();
    }

    public bool GetAllProjects(out List<Project> projects)
    {
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
    }

    public bool GetAllProjects(long managerId, out List<Project> projects)
    {
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ManagerId = {managerId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
    }

    public void DeleteOwnedProjects(long managerId)
    {
        List<Project> results;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ManagerId = {managerId}");

        DatabaseAccess.Instance.ExecuteQuery(query, out results);

        foreach (var project in results)
        {
            DeleteProject(project.ProjectId);
        }
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

        return success;
    }

    public bool UpdateProject(Project data)
    {
        List<Project> projects;
        var query = FormattableStringFactory.Create($"UPDATE dbo.Projects SET ManagerId = {data.ManagerId}, Name = '{data.Name}', Description = '{data.Description}' WHERE ProjectId = {data.ProjectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        return success;
    }

    private void DeleteAllMembers(long projectId)
    {
        List<ProjectMembers> data;
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.ProjectMembers WHERE ProjectId = {projectId}");

        DatabaseAccess.Instance.ExecuteQuery(query, out data);
    }

    public bool DeleteProject(long projectId)
    {
        List<Meeting> meetings;
        List<Project> projects;

        TaskListManager.Instance.DeleteTaskLists(projectId);
        UserManager.Instance.DeleteAllPermissions(projectId);
        DeleteAllMembers(projectId);

        var query = FormattableStringFactory.Create($"DELETE FROM dbo.Projects WHERE ProjectId = {projectId}");

        Project data;
        bool success = GetProject(projectId, out data);
        bool success1 = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        return success && success1;
    }

    private bool ProjectExists(string name)
    {
        List<Project> projects;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE Name = '{name}'");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects)
            && projects != null && projects.Count != 0;

        return success;
    }

    public bool AcceptInvite(long projectId, long userId)
    {
        List<ProjectMembers> members;
        var query = FormattableStringFactory.Create($"UPDATE ProjectMembers SET InviteAccepted = 1 WHERE ProjectId = {projectId} AND UserId = {userId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out members);
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
        List<UserPermissions> temp;
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.ProjectMembers (ProjectId, UserId, InviteAccepted) VALUES ({projectId}, {userId}, 0)");
        var query1 = FormattableStringFactory.Create($"INSERT INTO dbo.UserPermissions (ProjectId, UserId, Editing, InvitingMembers, KickingMembers) VALUES ({projectId}, {userId}, 0, 0, 0)");

        return DatabaseAccess.Instance.ExecuteQuery(query, out data) && DatabaseAccess.Instance.ExecuteQuery(query1, out temp);
    }

    public bool RemoveProjectMember(Project project, User user)
    {
        List<ProjectMembers> data;
        List<UserPermissions> temp;
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.UserPermissions WHERE ProjectId = {project.ProjectId} AND UserId = {user.UserId}");
        var query1 = FormattableStringFactory.Create($"DELETE FROM dbo.ProjectMembers WHERE ProjectId = {project.ProjectId} AND UserId = {user.UserId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out temp) && DatabaseAccess.Instance.ExecuteQuery(query1, out data);

        if (!success)
        {
            return false;
        }

        var subject = "Manage IT Notification: You have been kicked from a project";
        var body = $"Dear {user.Login},<br/>Unfortunately You have been kicked from a project named {project.Name}. If You don't agree with this decision, contact the manager or an administrator!";
        string error;

        return EmailService.SendEmail(user.Email, subject, body, out error);
    }
}