using Web;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;

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

        EmailService.SendEmail(user.Email, "Manage IT Account Confirmation", "Your account has been created.", out error);
        return true;
    }

    public bool LoginUser(User user)
    {
        List<User> users;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE (Email LIKE '{user.Email}' OR Login LIKE '{user.Login}') AND Password LIKE '{user.Password}' AND Verified = 1");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);
        success = success && users != null && users.Count != 0;

        if (success)
        {
            CurrentSessionUser = users[0];

            var username = CurrentSessionUser.Login;
            var dateTime = DateTime.Now;
            var successful = success ? "successful" : "failed";
            var subject = "Alert: New Login Attempt On Your Account!";
            var body = string.Format("Dear {0}, \nWe noticed a {1} login attempt on your account on {2}. \nIf this was you, no further action is needed. \nIf you did not attempt to log in, please secure your account immediately by changing your password.\nSincerely\nManage IT Team.", username, successful, dateTime.ToString());
            string error;

            EmailService.SendEmail(CurrentSessionUser.Email, subject, body, out error);
        }

        return success;
    }

    public bool VerifyUser(string email)
    {
        List<User> users;
        var checkVerificationQuery = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{email}' AND Verified = 1");
        bool verified = DatabaseAccess.Instance.ExecuteQuery(checkVerificationQuery, out users) && users != null && users.Count != 0;

        if (verified)
        {
            return false;
        }

        var query = FormattableStringFactory.Create($"UPDATE dbo.Users SET Verified = 1 WHERE Email LIKE '{email}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);
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
