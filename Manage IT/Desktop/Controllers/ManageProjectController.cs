using EFModeling.EntityProperties.DataAnnotations.Annotations;

public static class ManageProjectController
{
    public static void SubmitEditForm(long projectId, long managerId, string name, string description, out string error)
    {
        Project data = new();
        data.ProjectId = projectId;
        data.ManagerId = managerId;
        data.Name = name;
        data.Description = description;

        bool success = ProjectManager.Instance.UpdateProject(data);

        if (!success)
        {
            error = "Could not edit the project!";
            return;
        }

        error = "";
    }

    public static void SubmitDeleteForm(long projectId, out string error)
    {
        bool success = ProjectManager.Instance.DeleteProject(projectId);

        if (!success)
        {
            error = "Could not delete the project!";
            return;
        }

        error = "";
    }
}
