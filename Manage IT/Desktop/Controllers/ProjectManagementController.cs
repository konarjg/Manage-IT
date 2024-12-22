using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ProjectManagementController
{
    public static void FetchProjectList(long userId, ref List<Project> projects, out string error)
    {
        bool success = ProjectManager.Instance.GetAllProjects(userId, out projects);

        if (success)
        {
            error = "";
            return;
        }

        error = "Couldn't load projects!";
    }
}
