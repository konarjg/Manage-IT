using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EFModeling.EntityProperties.DataAnnotations.Annotations;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        private Regex EmailValidation = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}");
        private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");

        private User CurrentLoginData;
        private int CurrentConfirmCode;

        public void SwitchPageTemplate(string name)
        {
            Template = Resources[name] as ControlTemplate;
        }

        private T GetTemplateControl<T>(string name) where T : class
        {
            return Template.FindName(name, this as FrameworkElement) as T;
        }

        private void SubmitRegisterForm(out User data)
        {
            var email = GetTemplateControl<TextBox>("Email").Text;
            var username = GetTemplateControl<TextBox>("Username").Text;
            var password = GetTemplateControl<PasswordBox>("Password").Password;
            var confirmPassword = GetTemplateControl<PasswordBox>("ConfirmPassword").Password;

            if (email == string.Empty || username == string.Empty
                || password == string.Empty || confirmPassword == string.Empty)
            {
                throw new Exception("You have to fill in every field!");
            }

            if (password != confirmPassword)
            {
                throw new Exception("Passwords aren't identical!");
            }

            if (!EmailValidation.IsMatch(email))
            {
                throw new Exception("Provided email is invalid!");
            }

            if (PasswordValidation.IsMatch(password))
            {
                throw new Exception("Password must be at least 8 characters long, contain at least 1 special character, at least 1 uppercase letter and at least 1 number!");
            }

            password = Security.HashText(password, Encoding.UTF8);

            data = new();
            data.Email = email;
            data.Login = username;
            data.Password = password;
        }

        private void SubmitLoginForm(out User data)
        {
            var credential = GetTemplateControl<TextBox>("Credential").Text;
            var password = GetTemplateControl<PasswordBox>("Password").Password;

            if (credential == string.Empty || password == string.Empty)
            {
                throw new Exception("You have to fill in every field!");
            }

            password = Security.HashText(password, Encoding.UTF8);

            data = new();
            data.Email = credential;
            data.Login = credential;
            data.Password = password;
        }

        public MainWindow()
        {
            InitializeComponent();
            SwitchPageTemplate("Main");
        }

        public void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (App.Instance.UserSettings == null)
            {
                return;
            }

            if (!App.Instance.UserSettings.RememberMe)
            {
                return;
            }

            ProjectManagementWindow window = new();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Register");
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Login");
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Main");
        }


        public void ForgotPasswordClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("ForgotPassword");
        }

        public void ForgotPasswordBackClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Login");
        }

        private void SubmitRegisterFormClick(object sender, RoutedEventArgs e) 
        {
            User data;
            GetTemplateControl<TextBlock>("Error").Foreground = Brushes.Red;

            try
            {
                var error = string.Empty;
                SubmitRegisterForm(out data);
                RegisterController.SubmitRegisterForm(data, out error);

                if (error == string.Empty)
                {
                    GetTemplateControl<TextBlock>("Error").Foreground = Brushes.White;
                    GetTemplateControl<TextBlock>("Error").Text = "Verify your email address!";
                    return;
                }

                GetTemplateControl<TextBlock>("Error").Text = error;
            }
            catch (Exception error)
            {
                GetTemplateControl<TextBlock>("Error").Text = error.Message;
            }
        }

        public void SubmitLoginFormClick(object sender, RoutedEventArgs e)
        {
            User data;

            try
            {
                SubmitLoginForm(out data);
                User user;

                if (!UserManager.Instance.UserExists(data, out user) || user == null || data.Password != user.Password)
                {
                    GetTemplateControl<TextBlock>("Error").Text = "Provided credentials are invalid!";
                    return;
                }

                var error = string.Empty;
                App.Instance.LoadUserSettings(user);
                var settings = App.Instance.UserSettings;

                if (settings.Enable2FA)
                {
                    var code = Random.Shared.Next(1000, 10000);
                    var subject = "Alert: New Login Attempt On Your Account";
                    var body = $"A new login attempt was detected on Your account<br/>If this was You, type the following code in desktop application to confirm: {code}.<br/>If this wasn't You, change password immediately!";

                    EmailService.SendEmail(user.Email, subject, body, out error);

                    if (error != string.Empty)
                    {
                        MessageBox.Show("There was an unexpected error!");
                        return;
                    }

                    CurrentConfirmCode = code;
                    CurrentLoginData = data;
                    SwitchPageTemplate("ConfirmLogin");
                    return;
                }

                LoginController.SubmitLoginForm(data, out error);
                GetTemplateControl<TextBlock>("Error").Text = error;

                if (error != "Logged in!")
                {
                    return;
                }

                var window = new ProjectManagementWindow();
                window.Activate();
                window.Visibility = Visibility.Visible;

                App.Instance.UserSettings.UserData = new(UserManager.Instance.CurrentSessionUser);
                Close();
            }
            catch (Exception error)
            {
                GetTemplateControl<TextBlock>("Error").Text = error.Message;
            }
        }

        public void ForceDigit(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9+]");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void DigitFilled(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;

            if (box.Text == "")
            {
                return;
            }

            var request = new TraversalRequest(FocusNavigationDirection.Next);
            box.MoveFocus(request);
        }

        public void ConfirmLoginBackClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Login");
        }


        public void ConfirmLoginClick(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new();

            for (int i = 1; i <= 4; i++)
            {
                builder.Append(GetTemplateControl<TextBox>($"Digit{i}").Text);
            }

            var code = int.Parse(builder.ToString());

            if (code != CurrentConfirmCode)
            {
                GetTemplateControl<TextBlock>("Error").Text = "Invalid verification code!";
                return;
            }

            string error;
            LoginController.SubmitLoginForm(CurrentLoginData, out error);

            if (error != "Logged in!")
            {
                GetTemplateControl<TextBlock>("Error").Text = error;
                return;
            }

            var window = new ProjectManagementWindow();
            window.Activate();
            window.Visibility = Visibility.Visible;

            App.Instance.UserSettings.UserData = new(UserManager.Instance.CurrentSessionUser);
            Close();
        }

        private void SubmitForgotPasswordForm(out User data)
        {
            var credential = GetTemplateControl<TextBox>("Credential").Text;
            var password = GetTemplateControl<PasswordBox>("Password").Password;
            var confirmPassword = GetTemplateControl<PasswordBox>("ConfirmPassword").Password;

            if (credential == string.Empty || password == string.Empty || confirmPassword == string.Empty)
            {
                throw new Exception("You have to fill in every field!");
            }

            if (password != confirmPassword)
            {
                throw new Exception("Passwords aren't identical!");
            }

            if (PasswordValidation.IsMatch(password))
            {
                throw new Exception("Password must be at least 8 characters long, contain at least 1 special character, at least 1 uppercase letter and at least 1 number!");
            }

            password = Security.HashText(password, Encoding.UTF8);
            data = new();
            data.Login = credential;
            data.Email = credential;
            data.Password = password;
        }

        public void SubmitForgotPasswordFormClick(object sender, RoutedEventArgs e)
        {
            User data;
            GetTemplateControl<TextBlock>("Error").Foreground = Brushes.Red;

            try
            {
                var error = string.Empty;
                SubmitForgotPasswordForm(out data);
                ForgotPasswordController.SubmitForgotPasswordForm(data, out error);

                if (error == string.Empty)
                {
                    GetTemplateControl<TextBlock>("Error").Foreground = Brushes.White;
                    GetTemplateControl<TextBlock>("Error").Text = "Password restoration email has been sent";
                    return;
                }

                GetTemplateControl<TextBlock>("Error").Text = error;
            }
            catch (Exception error)
            {
                GetTemplateControl<TextBlock>("Error").Foreground = Brushes.Red;
                GetTemplateControl<TextBlock>("Error").Text = error.Message;
            }
        }

    }
}
