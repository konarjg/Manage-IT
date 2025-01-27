using EFModeling.EntityProperties.DataAnnotations.Annotations;

public static class CreateProjectController
{
    public static void SubmitCreateProjectForm(string name, string description, out string error)
    {
        Project data = new();
        data.Name = name;
        data.Description = description;
        data.ManagerId = UserManager.Instance.CurrentSessionUser.UserId;

        bool success = ProjectManager.Instance.CreateProject(data);

        if (success)
        {
            error = "";
            return;
        }

        error = "Could not create the project!";
    }
}
