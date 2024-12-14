using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

public class ProjectManager
{
    public static ProjectManager Instance { get; private set; }
    
    public static void Instantiate()
    {
        Instance = new ProjectManager();
    }

    public bool GetAllProjects(int managerId, out List<Project> projects)
    {
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Projects WHERE ManagerId = {managerId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out projects);
    }

    public bool CreateProject(Project data)
    {
        List<Project> projects;
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.Projects (Name, Description, ManagerId) VALUES ('{data.Name}', '{data.Description}', {data.ManagerId})");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out projects);

        return success;
    }
}