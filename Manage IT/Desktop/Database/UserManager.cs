using Desktop;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
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

        var success = DatabaseAccess.Instance.ExecuteQuery(query, out users);
        
        if (!success)
        {
            error = "There was an unexpected error! Could not create an account.";
            return false;
        }

        var subject = "Manage IT Account Confirmation";
        var username = user.Login;
        var url = string.Format("http://manageit.runasp.net/VerifyEmail?emailEncrypted={0}", Security.EncryptText(user.Email));
        //var url = string.Format("https://localhost:5001/VerifyEmail?emailEncrypted={0}", Security.EncryptText(user.Email));
        var body = string.Format("Dear {0},<br>Thank You for choosing Manage IT. <br><a href=\"{1}\">Click here to verify your account</a>", username, url);

        EmailService.SendEmail(user.Email, subject, body, out error);
        error = string.Empty;
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

            MessageBox.Show(users[0].Email);

            var username = CurrentSessionUser.Login;
            var dateTime = DateTime.Now;
            var successful = success ? "successful" : "failed";
            var subject = "Alert: New Login Attempt On Your Account!";
            var body = string.Format("Dear {0}, <br>We noticed a {1} login attempt on Your account on {2}. <br>If this was You, no further action is needed. <br>If You did not attempt to log in, please secure Your account immediately by changing Your password.<br>Sincerely<br>Manage IT Team.", username, successful, dateTime.ToString());
            string error;

            EmailService.SendEmail(CurrentSessionUser.Email, subject, body, out error);
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
