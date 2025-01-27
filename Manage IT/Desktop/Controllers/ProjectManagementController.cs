using Desktop;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;

public static class ProjectManagementController
{
    public static void FetchProjectList(long userId, ref List<Project> projects, DisplayProjects display, out string error)
    {
        bool success = false;

        switch (display)
        {
            case DisplayProjects.All:
                success = ProjectManager.Instance.GetAllProjects(userId, out projects);
                break;

            case DisplayProjects.Owned:
                success = ProjectManager.Instance.GetOwnedProjects(userId, out projects);
                break;

            case DisplayProjects.Shared:
                success = ProjectManager.Instance.GetSharedProjects(userId, out projects);
                break;
        }

        if (success)
        {
            error = "";
            return;
        }

        error = "Couldn't load projects!";
    }
}
