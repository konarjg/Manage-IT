public class UserManager
{
  User CurrentSessionUser;

  private void LoginUser(string login, string password) {

  }

  private void LoginUser(string email, string password) {

  }

  private void RegisterUser(string login, string email, string password, Prefix prefix, int phoneNumber){

  }

  private void ChangeCredentials(string newLogin, string newEmail, string newPassword, Prefix newPrefix, int newPhoneNumber) {

  }

  private void RemoveAccount(string email, string password){
      $password = mysqli_query("SELECT * FROM Users WHERE email = " + email + "",database);
        if()
        "DELETE FROM Users WHERE 
  }

  private void RemoveAccount(string login, string password){

  }
}
