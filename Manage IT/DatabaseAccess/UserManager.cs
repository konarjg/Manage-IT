using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Runtime.CompilerServices;

public class UserManager
{
    public static User CurrentSessionUser { get; private set; }

    public static UserManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new UserManager();
    }

    public bool RegisterUser(User user, out string error)
    {
        if (UserExists(user))
        {
            error = "Account with this email already exists!";
            return false;
        }

        List<User> users;
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.Users (Login,Password,Email,PrefixId,PhoneNumber) VALUES ('{user.Login}', '{user.Password}','{user.Email}',{user.PrefixId},'{user.PhoneNumber}')");

        error = "";
        var success = DatabaseAccess.Instance.ExecuteQuery(query, out users);
        
        if (!success)
        {
            error = "There was an unexpected error! Could not create an account.";
            return false;
        }

        return true;
    }

    public bool LoginUserViaUsername(User user)
    {
        List<User> users;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Login LIKE '{user.Login}' AND Password LIKE '{user.Password}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users)
            && users != null && users.Count != 0;

        if (success)
        {
            CurrentSessionUser = user;
        }

        return success;
    }

    public bool LoginUserViaEmail(User user)
    {
        List<User> users;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{user.Email}' AND Password LIKE '{user.Password}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users) 
            && users != null && users.Count != 0;

        if (success)
        {
            CurrentSessionUser = user;
        }

        return success;
    }

    private bool UserExists(User user)
    {
        List<User> existingUsers;
        var queryUserExists = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{user.Email}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(queryUserExists, out existingUsers);

        if (existingUsers == null || !success)
        {
            return false;
        }

        return existingUsers.Count != 0;
    }
}
