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
        List<User> users;
        var query = FormattableStringFactory.Create("INSERT INTO dbo.Users (Login,Password,Email,PrefixId,PhoneNumber) VALUES ('{0}', '{1}','{2)',{3},{4})", user.Login, user.Password, user.Email, user.PrefixId, user.PhoneNumber);
        return AccessDatabase.Instance.ProcessQuery(query, out users);
    }

    public bool LoginUser(User user)
    {
        List<User> user;
        var queryLogin = FormattableStringFactory.Create("SELECT * FROM dbo.Users WHERE Login = '{0}'",user.Login);
        var queryEmail = FormattableStringFactory.Create("SELECT * FROM dbo.Users WHERE Email = '{0}'",user.Email);
        bool success = DatabaseAccess.Instance.ProcessQuery(queryLogin, out user) || DatabaseAccess.Instance.ProcessQuery(queryEmail, out user);
        return false;
    }

    private bool UserExists(User user)
    {
        List<User> existingUser;
        var queryUserExists = FormattableStringFactory.Create("SELECT * FROM dbo.Users WHERE Login = '{0}'",user.Login);
        bool success = DatabaseAccess.Instance.ProcessQuery(queryUserExists, out existingUser);
        return existingUser == null && existingUser.Count() == 0;
    }
}
