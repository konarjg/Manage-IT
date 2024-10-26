using EFModeling.EntityProperties.DataAnnotations.Annotations;

public class ProjectManager
{
    public Project CurrentProject { get; private set; }
    public static ProjectManager Instance { get; private set; }
    
    public static void Instantiate()
    {

    }
}