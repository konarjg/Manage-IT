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
    public partial class CreateProjectWindow : Window
    {
        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        public void SubmitCreateProjectFormClick(object sender, RoutedEventArgs e)
        {
            var name = ProjectName.Text;
            var description = Description.Text;

            if (name == null || name == "" || description == null || description == "")
            {
                Error.Text = "You have to fill in every field!";
                return;
            }

            string error;
            CreateProjectController.SubmitCreateProjectForm(name, description, out error);

            if (error == "")
            {
                ProjectManagementWindow projectManagement = new();
                projectManagement.Activate();
                projectManagement.Visibility = Visibility.Visible;
                Close();
                return;
            }

            Error.Text = error;
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {
            ProjectManagementWindow projectManagement = new();
            projectManagement.Activate();
            projectManagement.Visibility = Visibility.Visible;
            Close();
        }
    }
}
