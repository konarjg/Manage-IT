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
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
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
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ManagerId = {managerId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
    }

    public bool GetSharedProjects(long userId, out List<Project> projects)
    {
        FormattableString query = FormattableStringFactory.Create($"SELECT p.* FROM dbo.Projects p JOIN dbo.ProjectMembers pm ON p.ProjectId = pm.ProjectId WHERE pm.UserId = {userId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
    }

    public void DeleteOwnedProjects(long managerId)
    {
        List<Project> results;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ManagerId = {managerId}");

        DatabaseAccess.Instance.ExecuteQuery(query, out results);

        foreach (Project project in results)
        {
            DeleteProject(project.ProjectId);
        }
    }


    public bool GetProject(long projectId, out Project project)
    {
        List<Project> projects;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ProjectId = {projectId}");

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
        FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.Projects (Name, Description, ManagerId) VALUES ('{data.Name}', '{data.Description}', {data.ManagerId})");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        if (success)
        {
            UserManager.Instance.CreatePermissions(data.ManagerId, data.Name);
        }

        return success;
    }

    public bool UpdateProject(Project data)
    {
        List<Project> projects;
        FormattableString query = FormattableStringFactory.Create($"UPDATE dbo.Projects SET ManagerId = {data.ManagerId}, Name = '{data.Name}', Description = '{data.Description}' WHERE ProjectId = {data.ProjectId}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        return success;
    }

    private void DeleteAllMembers(long projectId)
    {
        List<ProjectMembers> data;
        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.ProjectMembers WHERE ProjectId = {projectId}");

        DatabaseAccess.Instance.ExecuteQuery(query, out data);
    }

    public bool DeleteProject(long projectId)
    {
        List<Project> projects;

        MeetingManager.Instance.DeleteAllMeetings(projectId);
        TaskListManager.Instance.DeleteTaskLists(projectId);
        UserManager.Instance.DeleteAllPermissions(projectId);
        DeleteAllMembers(projectId);

        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Projects WHERE ProjectId = {projectId}");

        Project data;
        bool success = GetProject(projectId, out data);
        bool success1 = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        return success && success1;
    }
    public bool GetProjectMembers(long projectId, out List<User> members)
    {
        FormattableString query = FormattableStringFactory.Create($@"
        SELECT U.* 
        FROM dbo.Users U 
        INNER JOIN dbo.ProjectMembers PM ON U.UserId = PM.UserId 
        WHERE PM.ProjectId = {projectId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out members);
    }
    public bool IsProjectMember(long projectId, long userId)
    {
        FormattableString query = FormattableStringFactory.Create($@"
        SELECT *
        FROM dbo.ProjectMembers 
        WHERE ProjectId = {projectId} AND UserId = {userId}");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out List<ProjectMembers> user);

        if (user != null)
        {
            if (user.Count() > 0)
            {
                return user[0].InviteAccepted;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    private bool ProjectExists(string name)
    {
        List<Project> projects;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE Name = '{name}'");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects)
            && projects != null && projects.Count != 0;

        return success;
    }
    public bool GetInviteStatus(long projectID, long userID)
    {
        List<ProjectMembers> data;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.ProjectMembers WHERE UserId = {userID} AND ProjectId = {projectID}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out data);

        if (success)
        {
            return data[0].InviteAccepted;
        }
        return false;
    }


    public bool AcceptInvite(long projectId, long userId)
    {
        List<ProjectMembers> members;
        FormattableString query = FormattableStringFactory.Create($"UPDATE ProjectMembers SET InviteAccepted = 1 WHERE ProjectId = {projectId} AND UserId = {userId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out members);
    }
    public bool AddProjectMember(long projectId, long userId)
    {
        List<ProjectMembers> data;
        List<UserPermissions> temp;
        FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.ProjectMembers (ProjectId, UserId, InviteAccepted) VALUES ({projectId}, {userId}, 0)");
        FormattableString query1 = FormattableStringFactory.Create($"INSERT INTO dbo.UserPermissions (ProjectId, UserId, Editing, InvitingMembers, KickingMembers) VALUES ({projectId}, {userId}, 0, 0, 0)");

        return DatabaseAccess.Instance.ExecuteQuery(query, out data) && DatabaseAccess.Instance.ExecuteQuery(query1, out temp);
    }

    public bool RemoveProjectMember(Project project, User user)
    {
        List<ProjectMembers> data;
        List<UserPermissions> temp;
        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.UserPermissions WHERE ProjectId = {project.ProjectId} AND UserId = {user.UserId}");
        FormattableString query1 = FormattableStringFactory.Create($"DELETE FROM dbo.ProjectMembers WHERE ProjectId = {project.ProjectId} AND UserId = {user.UserId}");

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
}
