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
        user.Login, user.Password, user.Email, user.Prefix, user.Phone;
        //INSERT INTO Users(Login,Password,Email,Prefix,PhoneNumber) VALUES...
         var query = FormattableStringFactory.Create("INSERT INTO dbo.Users (Login,Password,Email,Prefix,PhoneNumber) VALUES ('{0}', '{1}','{2)',{3},{4})", Login, Password, Email, Prefix, PhoneNumber);
        return false;
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
