using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Reflection.PortableExecutable;
using System.Diagnostics.Eventing.Reader;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Desktop
{
    /// <summary>
    /// Logika interakcji dla klasy AdminPanelWindowMain.xaml
    /// </summary>
    public partial class AdminPanelWindowMain : Window
    {
        private Regex EmailValidation = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}");
        private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");
        private Stack<ControlTemplate> templateHistory;
        private UserSettings CurrentSettings;
        private User userToModify;
        public AdminPanelWindowMain()
        {
            templateHistory = new Stack<ControlTemplate>();
            CurrentSettings = new(App.Instance.UserSettings);
            InitializeComponent();
        }
        private T GetTemplateControl<T>(string name) where T : class
        {
            return Template.FindName(name, this as FrameworkElement) as T;
        }

        private void SwitchPageTemplate(string name)
        {
            if (Template != null)
            {
                templateHistory.Push(Template); // Zapamiętaj aktualny szablon
            }

            var newTemplate = Resources[name] as ControlTemplate;
            if (newTemplate != null)
            {
                Template = newTemplate;

            }
            else
            {
                MessageBox.Show($"Template '{name}' not found in resources.");
            }

        }
        private void ReplaceUserNameInText(object sender, RoutedEventArgs e)
        {
            if (userToModify != null)
            {
                string textToReplace = "(user.name)";
                string userName = userToModify.Login;

                if (sender is TextBlock textBlock)
                {
                    textBlock.Text = textBlock.Text.Replace(textToReplace, userName);
                }
            }
        }

        private void SwitchBackToPreviousTemplate()
        {

            if (templateHistory.Count > 0)
            {
                Template = templateHistory.Pop(); // Przywróć poprzedni szablon
                TextBlock header = GetTemplateControl<TextBlock>("ProjectNameHeader");
                if (header != null)
                    header.Text = "Project name";
            }
            else
            {
                var window = new ProjectManagementWindow();
                window.Activate();
                window.Visibility = Visibility.Visible;

                Close();
            }

        }
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject { if (depObj != null) { for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) { DependencyObject child = VisualTreeHelper.GetChild(depObj, i); if (child != null && child is T) { yield return (T)child; } foreach (T childOfChild in FindVisualChildren<T>(child)) { yield return childOfChild; } } } }
        private void ResetTextInputs()
        {
            var allTextBoxes = FindVisualChildren<TextBox>(this);
            var allPasswordBoxes = FindVisualChildren<PasswordBox>(this);
            foreach (var textBox in allTextBoxes)
            {
                textBox.Text = string.Empty;
            }
            foreach (var passwordBox in allPasswordBoxes)
            {
                passwordBox.Clear();
            }
        }
        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("LogOut");
        }

        public void CancelLogOutClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }
        public void CancelDeleteClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }
        public void ConfirmDeleteClick(object sender, RoutedEventArgs e)
        { 
            bool success = UserManager.Instance.DeleteUser(userToModify);
            if (!success)
            {
                TextBlock error = GetTemplateControl<TextBlock>("Error");
                error.Text = "There was an unexpected error!";
            }
            userToModify = null;
            UsersClick(sender, e);
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
        public void UsersListViewLoaded(object sender, RoutedEventArgs e)
        {
            string error = String.Empty;
            bool success = UserManager.Instance.GetAllUsers(out List<User> users);

            if (!success || !string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return;
            }

            StackPanel membersListPanel = sender as StackPanel; // Cast sender to StackPanel
            if (membersListPanel == null)
            {
                MessageBox.Show("Sender is not a StackPanel");
                return;
            }

            int i = 0;
            foreach (var user in users)
            {
                Button button = new Button();
                button.Content = "id:" + user.UserId + " | " + user.Login;
                button.Name = $"Member{user.UserId}";
                //button.CommandParameter = user.UserId;

                // Use a lambda to forward the user ID to the ModifyUserClick method
                if (user.UserId == UserManager.Instance.CurrentSessionUser.UserId)
                {
                    button.Click += SwitchToUserSettings;
                }
                else
                {
                    button.Click += (s, args) => ModifyUserClick(s, args, user.UserId);
                }

                button.Style = (Style)FindResource("ListButton");

                membersListPanel.Children.Insert(0, button); // Adding to StackPanel's children
            }
        }


        public void ModifyUserClick(object sender, RoutedEventArgs e, long userId)
        {
            bool success = UserManager.Instance.GetUser(userId, out User user);
            if(success)
            {
                userToModify = user;
                SwitchPageTemplate("UserEdit");
            }
            
        }

        public void FillWithCredential(object sender, RoutedEventArgs e)
        {
            // Check if the sender is a TextBox
            if (sender is TextBox textBox && userToModify!=null)
            {
                // Fetch the user (assuming you have the user already fetched and stored in a variable)
                User user = userToModify;

                // Check the tag of the TextBox and fill it with the appropriate user information
                if (textBox.Tag != null)
                {
                    switch (textBox.Tag.ToString())
                    {
                        case "Login":
                            textBox.Text = user.Login;
                            break;
                        case "Email":
                            textBox.Text = user.Email;
                            break;
                        default:
                            textBox.Text = "Unknown Tag";
                            break;
                    }
                }
            }
        }



        public void SwitchToUserSettings(object sender, RoutedEventArgs e)
        {
            UserSettingsWindow window = new(true);
            window.Activate();
            window.Visibility = Visibility.Visible;
            Close();
           
        }
        public void ProjectListViewLoaded(object sender, RoutedEventArgs e)
        {
            string error = String.Empty;
            bool success = ProjectManager.Instance.GetAllProjects(out List<Project> projects);

            if (!success || !string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return;
            }

            StackPanel membersListPanel = sender as StackPanel; // Cast sender to StackPanel
            if (membersListPanel == null)
            {
                MessageBox.Show("Sender is not a StackPanel");
                return;
            }

            int i = 0;
            foreach (var project in projects)
            {
                Button button = new Button();
                button.Content = "id:" + project.ProjectId + " | " + project.Name;
                button.Name = $"Project{project.ProjectId}";
                button.Click += SwitchToProjectAdminPanel;
                button.Tag = project.ProjectId;
                button.Style = (Style)FindResource("ListButton");

                membersListPanel.Children.Insert(0, button); // Adding to StackPanel's children
            }
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {
            //prolly if in main adm panel window then do this.close()
            SwitchBackToPreviousTemplate();
        }
        public void SwitchToProjectAdminPanel(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && long.TryParse(button.Tag.ToString(), out long projectId))
            {
                Project project = ProjectManager.Instance.GetProjectById(projectId);
                var window = new AdminPanelWindow(project,false);
                window.Activate();
                window.Visibility = Visibility.Visible;

                Close();
            }
        }



        public void OverviewClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Overview");
        }

        public void UsersClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Users");
        }
        public void SecurityClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Security");
        }
        public void ProjectsClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Projects");
        }
        public void PutCurrentUserCredential(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBox)
            {
                switch (textBox.Tag)
                {
                    case "Login":
                        textBox.Text = $"{textBox.Text}{UserManager.Instance.CurrentSessionUser.Login}";
                        break;

                    case "Email":
                        textBox.Text = $"{textBox.Text}{UserManager.Instance.CurrentSessionUser.Email}";
                        break;

                    case "UserID":
                        textBox.Text = $"{textBox.Text}{UserManager.Instance.CurrentSessionUser.UserId}";
                        break;

                    default:
                        // Handle any other cases, if needed
                        break;
                }
            }
        }

        public void UserModifyReset(object sender, RoutedEventArgs e)
        {
            TextBox loginTextBox = GetTemplateControl<TextBox>("ChangeUserNameTextBox"); 
            loginTextBox.Text = userToModify.Login;
            TextBox emailTextBox = GetTemplateControl<TextBox>("ChangeEmailTextBox");
            emailTextBox.Text = userToModify.Email;
            PasswordBox newPasswordBox = GetTemplateControl<PasswordBox>("NewPassword");
            PasswordBox confirmPasswordBox = GetTemplateControl<PasswordBox>("ConfirmPassword");
            confirmPasswordBox.Password = String.Empty;
            newPasswordBox.Password = String.Empty;
        }
        public void UserModifyConfirm(object sender, RoutedEventArgs e)
        {
            TextBox loginTextBox = GetTemplateControl<TextBox>("ChangeUserNameTextBox");
            TextBox emailTextBox = GetTemplateControl<TextBox>("ChangeEmailTextBox");
            TextBlock error = GetTemplateControl<TextBlock>("Error");
            PasswordBox newPasswordBox = GetTemplateControl<PasswordBox>("NewPassword");
            PasswordBox confirmPasswordBox = GetTemplateControl<PasswordBox>("ConfirmPassword");

            if (emailTextBox.Text == string.Empty || loginTextBox.Text == string.Empty
                || newPasswordBox.Password == string.Empty || confirmPasswordBox.Password == string.Empty)
            {
                error.Text = "You have to fill in every field!";
                return;
            }

            if (newPasswordBox.Password != confirmPasswordBox.Password)
            {
                error.Text = "Passwords aren't identical!";
                return;
            }

            if (!EmailValidation.IsMatch(emailTextBox.Text))
            {
                error.Text = "Provided email is invalid!";
                return;
            }

            if (PasswordValidation.IsMatch(newPasswordBox.Password))
            {
                error.Text = "Password must be at least 8 characters long, contain at least 1 special character, at least 1 uppercase letter and at least 1 number!";
                return;
            }

            User data = new User();
            data.UserId = userToModify.UserId;
            data.Email = emailTextBox.Text;
            data.Login = loginTextBox.Text;
            data.Password = Security.HashText(newPasswordBox.Password, Encoding.UTF8);

            bool success = UserManager.Instance.UpdateUser(data);
            if (success)
            {
                userToModify = null;
                SwitchBackToPreviousTemplate();
            }
            else
            {
                error.Text = "There was an unexpected error!";
                return;
            }
            // Perform additional user modification logic here
        }


        public void DeleteAccountSeq(object sender, RoutedEventArgs e)
        {
            //UserManager.Instance.DeleteUser(userToModify);
            SwitchPageTemplate("UserDelete");
        }

        public void DisableAccountSeq(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                UserManager.Instance.DisableUser(userToModify);
                if (userToModify.Verified == false)
                {
                    button.Content = "This account is disabled/unverified";
                    button.IsEnabled = false;
                }
                else
                {
                    button.Content = "Disable Account";
                }
                //SwitchPageTemplate("UserEdit");
            }
        }

        public void AdjustDisableButton(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (userToModify.Verified == false)
                {
                    button.Content = "This account is disabled/unverified";
                    button.IsEnabled = false;
                }
                else
                {
                    button.Content = "Disable Account";
                }

            }
        }
        public void SettingsWindowReset(object sender, RoutedEventArgs e)
        {
            CurrentSettings = new(App.Instance.UserSettings);
            OnApplyTemplate();
        }
        public void SettingsWindowConfirm(object sender, RoutedEventArgs e)
        {
            //CurrentSettings = new(App.Instance.UserSettings);
            //OnApplyTemplate();
        }
    }
}
