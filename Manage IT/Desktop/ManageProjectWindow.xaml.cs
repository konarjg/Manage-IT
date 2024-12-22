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
    public partial class ManageProjectWindow : Window
    {
        public Project Project { get; private set; }
        private string TemplateKey { get; set; }

        private void SwitchPageTemplate(string name)
        {
            Template = Resources[name] as ControlTemplate;
            TemplateKey = name;
        }

        private T GetTemplateControl<T>(string name) where T : class
        {
            return Template.FindName(name, this as FrameworkElement) as T;
        }

        public ManageProjectWindow(Project project)
        {
            InitializeComponent();
            Project = project;

            if (Project == null)
            {
                MessageBox.Show("Unexpected error occured!");
                return;
            }

            SwitchPageTemplate("Main");
        }

        private void UpdateMainContent()
        {
            var projectName = GetTemplateControl<TextBlock>("ProjectName");

            if (projectName == null)
            {
                return;
            }

            projectName.Text = Project.Name;
        }

        private void UpdateProjectInfoContent()
        {
            var projectName = GetTemplateControl<TextBox>("ProjectName");
            var description = GetTemplateControl<TextBox>("Description");

            if (projectName == null || description == null)
            {
                return;
            }

            projectName.Text = Project.Name;
            description.Text = Project.Description;
        }

        private void UpdateEditContent()
        {
            var projectName = GetTemplateControl<TextBox>("ProjectName");
            var description = GetTemplateControl<TextBox>("Description");

            if (projectName == null || description == null)
            {
                return;
            }

            projectName.Text = Project.Name;
            description.Text = Project.Description;
        }

        private void UpdateDeleteContent()
        {
            var placeholder = GetTemplateControl<TextBlock>("ProjectNamePlaceholder");

            if (placeholder == null)
            {
                return;
            }

            placeholder.Text = Project.Name;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            switch (TemplateKey)
            {
                case "Main":
                    UpdateMainContent();
                    break;

                case "ProjectInfo":
                    UpdateProjectInfoContent();
                    break;

                case "Edit":
                    UpdateEditContent();
                    break;

                case "Delete":
                    UpdateDeleteContent();
                    break;
            }
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {
            ProjectManagementWindow window = new();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }

        public void ProjectInfoClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("ProjectInfo");
        }

        public void ProjectMembersClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("ProjectMembers");
        }

        public void MeetingClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Meeting");
        }

        public void EditClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Edit");
        }

        public void DeleteClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Delete");
        }

        public void PanelBackClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Main");
        }
        
        public void SubmitEditFormClick(object sender, RoutedEventArgs e)
        {
            var projectName = GetTemplateControl<TextBox>("ProjectName").Text;
            var description = GetTemplateControl<TextBox>("Description").Text;

            if (projectName == "" || description == "")
            {
                GetTemplateControl<TextBlock>("Error").Text = "Project name or description cannot be empty!";
                return;
            }

            string error;
            ManageProjectController.SubmitEditForm(Project.ProjectId, Project.ManagerId, projectName, description, out error);

            if (error == "")
            {
                Project.Name = projectName;
                Project.Description = description;
                SwitchPageTemplate("Main");
                return;
            }

            GetTemplateControl<TextBlock>("Error").Text = error;
        }

        public void SubmitDeleteFormClick(object sender, RoutedEventArgs e)
        {
            var projectName = GetTemplateControl<TextBox>("ProjectName").Text;

            if (projectName != Project.Name)
            {
                GetTemplateControl<TextBlock>("Error").Text = "You have to confirm the project name!";
                return;
            }

            string error;
            ManageProjectController.SubmitDeleteForm(Project.ProjectId, out error);

            if (error == "")
            {
                Project.Name = projectName;
                ProjectManagementWindow window = new();
                window.Activate();
                window.Visibility = Visibility.Visible;
                Close();
                return;
            }

            GetTemplateControl<TextBlock>("Error").Text = error;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            GetTemplateControl<TextBlock>("ProjectName").Text = Project.Name;
        }

        private void DeleteProjectNameTextChanged(object sender, TextChangedEventArgs e)
        {
            var projectName = GetTemplateControl<TextBox>("ProjectName");
            var placeholder = GetTemplateControl<TextBlock>("ProjectNamePlaceholder");

            if (projectName.Text == "")
            {
                placeholder.Visibility = Visibility.Visible;
            }
            else
            {
                placeholder.Visibility = Visibility.Collapsed;
            }
        }
    }
}
