using EFModeling.EntityProperties.DataAnnotations.Annotations;

public class UserManager
{
    public static User CurrentSessionUser { get; private set; }

    public static UserManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new UserManager();
    }

    public bool RegisterUser(User user)
    {
        //INSERT INTO Users(Login,Password,Email,Prefix,PhoneNumber) VALUES...
        List<User> users;
        var query = FormattableStringFactory.Create("INSERT INTO dbo.Users (Login,Password,Email,PrefixId,PhoneNumber) VALUES ('{0}', '{1}','{2)',{3},{4})", user.Login, user.Password, user.Email, user.PrefixId, user.PhoneNumber);
        return AccessDatabase.Instance.ProcessQuery(query, out users);
    }

    public bool LoginUser(User user)
    {
        //SELECT email,password FROM Users
        //SELECT login,password FROM Users
        return false;
    }

    private bool UserExists(User user)
    {
        //SELECT user FROM Users
        return false;
    }
}
