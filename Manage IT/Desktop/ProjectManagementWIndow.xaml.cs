using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Desktop
{
    public partial class ProjectManagementWindow : Window
    {
        private List<Project> Projects = new();

        public ProjectManagementWindow()
        {
            InitializeComponent();
        }
        private T GetTemplateControl<T>(string name) where T : class
        {
            return Template.FindName(name, this as FrameworkElement) as T;
        }

        public void ProjectManagementViewLoaded(object sender, RoutedEventArgs e)
        {
            string error;
            ProjectManagementController.FetchProjectList(UserManager.Instance.CurrentSessionUser.UserId, ref Projects, App.Instance.UserSettings.DisplayProjects, out error);

            

            if (error != "")
            {
                MessageBox.Show(error);
                return;
            }

            foreach (var project in Projects)
            {
                Button button = new();
                button.Content = project.Name;
                button.Name = $"Project{project.ProjectId}";
                button.Click += ProjectClick;

                if (project.ManagerId == UserManager.Instance.CurrentSessionUser.UserId)
                {
                    button.Style = (Style)FindResource("ProjectButton");
                }
                else
                {
                    button.Style = (Style)FindResource("SharedProjectButton");
                }

                ProjectList.Children.Insert(0, button);
            }
        }

        public void SettingsClick(object sender, RoutedEventArgs e)
        {
            UserSettingsWindow window = new(false);
            window.Activate();
            window.Visibility = Visibility.Visible;
            Close();
        }

        public void ProjectClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            long projectId;

            if (!long.TryParse(button.Name.Split("Project")[1], out projectId))
            {
                MessageBox.Show("Unexpected error occured!");
                return;
            }

            ManageProjectWindow window = new(Projects.Where(p => p.ProjectId == projectId).First());
            window.Activate();
            window.Visibility = Visibility.Visible;
            Close();
        }
        public void AdminPanelClick(object sender, RoutedEventArgs e)
        {
            var window = new AdminPanelWindowMain();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }
        public void CreateProjectClick(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow createForm = new();
            createForm.Activate();
            createForm.Visibility = Visibility.Visible;
            Close();
        }

        public void ChatClick(object sender, RoutedEventArgs e)
        {
            var window = new ChatWindow();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }

        public void CalendarClick(object sender, RoutedEventArgs e)
        {
            var window = new CalendarWindow();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }
<<<<<<< HEAD

        public void CheckForAdmin(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (UserManager.Instance.CurrentSessionUser.Admin)
                {
                    button.Visibility = Visibility.Visible;
                }
            }
        }

=======
>>>>>>> main
    }
}
