using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Desktop
{
    public enum DisplayProjects
    {
        All = 0,
        Owned = 1,
        Shared = 2
    }

    public class UserSettings
    {
        public User UserData { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DisplayProjects DisplayProjects { get; set; }
        public bool SendSecurityAlerts { get; set; }
        public bool SendProjectAlerts { get; set; }
        public bool RememberMe { get; set; }
        public bool Enable2FA { get; set; }

        public UserSettings() { }

        public UserSettings(UserSettings settings)
        {
            UserData = settings.UserData;
            DisplayProjects = settings.DisplayProjects;
            SendSecurityAlerts = settings.SendSecurityAlerts;
            SendProjectAlerts = settings.SendProjectAlerts;
            RememberMe = settings.RememberMe;
            Enable2FA = settings.Enable2FA;
        }
    }

    public class UserSettingsList
    {
        public List<UserSettings> UserSettings { get; set; }
    }


    public partial class UserSettingsWindow : Window
    {
        private string PreviousPage;
        private string CurrentPage;
        private UserSettings CurrentSettings;

        private Regex EmailValidation = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}");
        private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");

        public UserSettingsWindow()
        {
            InitializeComponent();
            SwitchPageTemplate("Account");
            PreviousPage = "Account";
            CurrentPage = PreviousPage;
            CurrentSettings = new(App.Instance.UserSettings);
        }

        private void SwitchPageTemplate(string name)
        {
            Template = Resources[name] as ControlTemplate;
        }

        private T GetTemplateControl<T>(string name) where T : class
        {
            return Template.FindName(name, this as FrameworkElement) as T;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            switch (CurrentPage)
            {
                case "Account":
                    TextBox changeUserNameText = GetTemplateControl<TextBox>("ChangeUserNameTextBox");
                    TextBox changeEmailText = GetTemplateControl<TextBox>("ChangeEmailTextBox");
                    PasswordBox newPassword = GetTemplateControl<PasswordBox>("NewPassword");
                    PasswordBox confirmPassword = GetTemplateControl<PasswordBox>("ConfirmPassword");
                    ToggleButton sendSecurityAlerts = GetTemplateControl<ToggleButton>("SendSecurityAlertsEmail");

                    changeUserNameText.Text = CurrentSettings.UserData.Login;
                    changeEmailText.Text = CurrentSettings.UserData.Email;
                    sendSecurityAlerts.IsChecked = CurrentSettings.SendSecurityAlerts;

                    break;

                case "Projects":
                    ListBox displayProjects = GetTemplateControl<ListBox>("DisplayProjectsBox");
                    ToggleButton sendProjectAlerts = GetTemplateControl<ToggleButton>("SendProjectAlertsEmailToggle");

                    displayProjects.SelectedIndex = (int)CurrentSettings.DisplayProjects;
                    sendProjectAlerts.IsChecked = CurrentSettings.SendProjectAlerts;
                    break;

                case "AdditionalSettings":
                    ToggleButton rememberMeToggle = GetTemplateControl<ToggleButton>("RememberMeToggle");
                    ToggleButton enable2FAToggle = GetTemplateControl<ToggleButton>("Enable2FAToggle");

                    rememberMeToggle.IsChecked = CurrentSettings.RememberMe;
                    enable2FAToggle.IsChecked = CurrentSettings.Enable2FA;
                    break;
            }
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {
            ProjectManagementWindow window = new ProjectManagementWindow();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }

        public void AccountClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Account");
            PreviousPage = "Account";
            CurrentPage = PreviousPage;
        }

        public void ProjectsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Projects");
            PreviousPage = "Projects";
            CurrentPage = PreviousPage;
        }

        public void AdditionalSettingsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("AdditionalSettings");
            PreviousPage = "AdditionalSettings";
            CurrentPage = PreviousPage;
        }

        public void SupportClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Support");
            PreviousPage = "Support";
            CurrentPage = PreviousPage;
        }

        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("LogOut");
            CurrentPage = "LogOut";
        }

        public void CancelLogOutClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate(PreviousPage);
        }

        public void ConfirmLogOutClick(object sender, RoutedEventArgs e)
        {
            CurrentSettings.RememberMe = false;
            App.Instance.SetUserSettings(CurrentSettings);
            App.Instance.SaveUserSettings();

            MainWindow window = new();
            window.Visibility = Visibility.Visible;
            window.Activate();
            window.SwitchPageTemplate("Login");

            Close();
        }

        public void DeleteAccountSeq(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("DeleteAccount");
            CurrentPage = "DeleteAccount";
        }

        public void DisableAccountSeq(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("DisableAccount");
            CurrentPage = "DisableAccount";
        }

        public void SettingsWindowReset(object sender, RoutedEventArgs e)
        {
            CurrentSettings = new(App.Instance.UserSettings);
            OnApplyTemplate();
        }

        public void DisplayProjectsChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox? listBox = sender as ListBox;

            if (listBox.SelectedItem == null)
            {
                listBox.SelectedIndex = 0;
            }
        }

        public void CancelDisableClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Account");
        }

        public void ConfirmDisableClick(object sender, RoutedEventArgs e)
        {
            bool success = UserManager.Instance.DisableUser(App.Instance.UserSettings.UserData);
            TextBlock errorDisplay = GetTemplateControl<TextBlock>("Error");

            if (!success)
            {
                errorDisplay.Text = "There was an unexpected error!";
                return;
            }

            errorDisplay.Foreground = Brushes.White;
            errorDisplay.Text = "Your account was disabled! If you wish to enable it again click the link sent to your email!";

            App.Instance.ResetSettings();
            MainWindow window = new();
            window.Visibility = Visibility.Visible;
            window.Activate();

            Close();
        }

        public void CancelDeleteClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Account");
        }

        public void ConfirmDeleteClick(object sender, RoutedEventArgs e)
        {
            bool success = UserManager.Instance.DeleteUser(App.Instance.UserSettings.UserData);
            TextBlock errorDisplay = GetTemplateControl<TextBlock>("Error");

            if (!success)
            {
                errorDisplay.Text = "There was an unexpected error!";
                return;
            }

            errorDisplay.Foreground = Brushes.White;
            errorDisplay.Text = "Your account was deleted! If you wish to restore it you either need to register again or contact an administrator!";

            App.Instance.ResetSettings();
            MainWindow window = new();
            window.Visibility = Visibility.Visible;
            window.Activate();

            Close();
        }

        #region ApplySettings

        private void ApplyAccountSettings()
        {
            string newUsername = GetTemplateControl<TextBox>("ChangeUserNameTextBox").Text;
            string newEmail = GetTemplateControl<TextBox>("ChangeEmailTextBox").Text;
            string newPassword = GetTemplateControl<PasswordBox>("NewPassword").Password;
            string confirmPassword = GetTemplateControl<PasswordBox>("ConfirmPassword").Password;
            bool? sendSecurityAlerts = GetTemplateControl<ToggleButton>("SendSecurityAlertsEmail").IsChecked;
            TextBlock errorDisplay = GetTemplateControl<TextBlock>("Error");

            if (newUsername == string.Empty)
            {
                newUsername = UserManager.Instance.CurrentSessionUser.Login;
            }

            if (newEmail == string.Empty)
            {
                newEmail = UserManager.Instance.CurrentSessionUser.Email;
            }
            else if (!EmailValidation.IsMatch(newEmail))
            {
                errorDisplay.Text = "Please enter a valid email address!";
                return;
            }

            if (newPassword == string.Empty)
            {
                newPassword = UserManager.Instance.CurrentSessionUser.Password;
            }
            else if (PasswordValidation.IsMatch(newPassword))
            {
                errorDisplay.Text = "Password must contain at least 8 characters, at least one uppercase letter, one lowercase letter and one special character!";
                return;
            }
            else
            {
                newPassword = Security.HashText(newPassword, Encoding.UTF8);
            }

            if (confirmPassword == string.Empty)
            {
                confirmPassword = UserManager.Instance.CurrentSessionUser.Password;
            }
            else
            {
                confirmPassword = Security.HashText(confirmPassword, Encoding.UTF8);
            }

            if (newPassword != confirmPassword)
            {
                errorDisplay.Text = "Passwords need to match!";
                return;
            }

            CurrentSettings.UserData.Login = newUsername;
            CurrentSettings.UserData.Email = newEmail;
            CurrentSettings.UserData.Password = newPassword;
            CurrentSettings.SendSecurityAlerts = sendSecurityAlerts != null ? (bool)sendSecurityAlerts : false;
        }

        private void ApplyProjectsSettings()
        {
            int displayProjects = GetTemplateControl<ListBox>("DisplayProjectsBox").SelectedIndex;
            bool? sendProjectAlerts = GetTemplateControl<ToggleButton>("SendProjectAlertsEmailToggle").IsChecked;

            CurrentSettings.DisplayProjects = (DisplayProjects)displayProjects;
            CurrentSettings.SendProjectAlerts = sendProjectAlerts != null ? (bool)sendProjectAlerts : false;
        }

        private void ApplyAdditionalSettings()
        {
            bool? rememberMe = GetTemplateControl<ToggleButton>("RememberMeToggle").IsChecked;
            bool? enable2FA = GetTemplateControl<ToggleButton>("Enable2FAToggle").IsChecked;

            CurrentSettings.RememberMe = rememberMe != null ? (bool)rememberMe : false;
            CurrentSettings.Enable2FA = enable2FA != null ? (bool)enable2FA : false;
        }

        #endregion

        public void SettingsWindowConfirm(object sender, RoutedEventArgs e)
        {
            TextBlock errorDisplay = GetTemplateControl<TextBlock>("Error");

            switch (CurrentPage)
            {
                case "Account":
                    ApplyAccountSettings();
                    break;

                case "Projects":
                    ApplyProjectsSettings();
                    break;

                case "AdditionalSettings":
                    ApplyAdditionalSettings();
                    break;
            }

            App.Instance.SetUserSettings(CurrentSettings);
            errorDisplay.Foreground = Brushes.White;
            errorDisplay.Text = "Settings successfully applied!";
        }

        public void SendSupportMessageClick(object sender, RoutedEventArgs e)
        {
            string topic = GetTemplateControl<TextBox>("TopicBox").Text;
            string message = GetTemplateControl<TextBox>("MessageBox").Text;
            TextBlock errorDisplay = GetTemplateControl<TextBlock>("Error");

            if (topic == string.Empty || message == string.Empty)
            {
                errorDisplay.Text = "Please fill in all required fields!";
                return;
            }

            string error;
            EmailService.SendEmail("manageitmail2024@gmail.com", topic, message, out error);

            if (error != string.Empty)
            {
                errorDisplay.Text = error;
            }

            errorDisplay.Foreground = Brushes.White;
            errorDisplay.Text = "Message successfully sent!";
        }
    }
}
