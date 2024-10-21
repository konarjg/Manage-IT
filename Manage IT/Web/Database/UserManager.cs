using EFModeling.EntityProperties.DataAnnotations.Annotations;

public class UserManager
{
    public static User CurrentSessionUser { get; private set; }

    public static UserManager Instance { get; private set; }

    public static void Instantiate()
    {

    }

    public bool RegisterUser(User user)
    {
        return false;
    }

    public bool LoginUser(User user)
    {
        return false;
    }

    private bool UserExists(User user)
    {
        return false;
    }
}
