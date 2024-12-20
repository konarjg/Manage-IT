using Web;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class UserManager
{
    public static User CurrentSessionUser { get; private set; }
    public static UserManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new UserManager();
    }

    public void ResetUser()
    {
        CurrentSessionUser = null;
    }

    public bool RegisterUser(User user, out string error)
    {
        if (UserExists(user))
        {
            error = "Account with this email already exists!";
            return false;
        }

        List<User> users;
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.Users (Login,Password,Email) VALUES ('{user.Login}', '{user.Password}','{user.Email}')");

        var success = DatabaseAccess.Instance.ExecuteQuery(query, out users);

        if (!success)
        {
            error = "There was an unexpected error! Could not create an account.";
            return false;
        }

        var subject = "Manage IT Account Confirmation";
        var username = user.Login;
        var url = string.Format("http://manageit.runasp.net/VerifyEmail?email={0}", user.Email);
        var body = string.Format("Dear {0},<br>Thank You for choosing Manage IT. <br><a href=\"{1}\">Click here to verify your account</a>", username, url);

        EmailService.SendEmail(user.Email, subject, body, out error);
        error = string.Empty;
        return true;
    }

    public bool LoginUser(User user)
    {
        User existingUser;

        if (!UserExists(user, out existingUser))
        {
            return false;
        }

        List<User> users;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE (Email LIKE '{user.Email}' OR Login LIKE '{user.Login}') AND Password LIKE '{user.Password}' AND Verified = 1");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out users);
        success = success && users != null && users.Count != 0;

        if (!success)
        {
            var username = existingUser.Login;
            var dateTime = DateTime.Now;
            var successful = "failed";
            var subject = "Alert: New Login Attempt On Your Account!";
            var body = string.Format("Dear {0}, <br>We noticed a {1} login attempt on Your account on {2}. <br>If this was You, no further action is needed. <br>If You did not attempt to log in, please secure Your account immediately by changing Your password.<br>Sincerely<br>Manage IT Team.", username, successful, dateTime.ToString());
            string error;

            EmailService.SendEmail(existingUser.Email, subject, body, out error);
        }
        else
        {
            CurrentSessionUser = users[0];
            var username = CurrentSessionUser.Login;
            var dateTime = DateTime.Now;
            var successful = "successful";
            var subject = "Alert: New Login Attempt On Your Account!";
            var body = string.Format("Dear {0}, <br>We noticed a {1} login attempt on Your account on {2}. <br>If this was You, no further action is needed. <br>If You did not attempt to log in, please secure Your account immediately by changing Your password.<br>Sincerely<br>Manage IT Team.", username, successful, dateTime.ToString());
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

    private bool UserExists(User data, out User user)
    {
        List<User> existingUsers;
        var queryUserExists = FormattableStringFactory.Create($"SELECT * FROM dbo.Users WHERE Email LIKE '{data.Email}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(queryUserExists, out existingUsers);

        if (existingUsers == null || !success || existingUsers.Count == 0)
        {
            user = null;
            return false;
        }

        user = existingUsers[0];
        return true;
    }
}
