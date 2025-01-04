using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using MessageBox = System.Windows.MessageBox;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

namespace Desktop
{
    public static class VisualTreeTraversal
    {
        public static DependencyObject FindName(DependencyObject parent, string controlName)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                var MyChild = VisualTreeHelper.GetChild(parent, i);
                if (MyChild is FrameworkElement && ((FrameworkElement)MyChild).Name == controlName)
                    return MyChild;

                var FindResult = FindName(MyChild, controlName);
                if (FindResult != null)
                    return FindResult;
            }

            return null;
        }
    }

    public partial class ManageProjectWindow : Window
    {
        public Project Project { get; private set; }
        public List<User> Members { get; private set; }
        private string TemplateKey { get; set; }
        private User CurrentKickedUser { get; set; }

        private DispatcherTimer Timer;

        public ObservableCollection<TaskList> TaskLists { get; set; } = new()
        {
            new()
            {
                Name="BRO"
            }
        };

        public ObservableCollection<Task> Tasks { get; set; } = new()
        {
            new()
            {
                Name="BRO"
            }
        };


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

            Timer = new()
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            Timer.Tick += UpdateMembersTick;

            LoadMembersList();
            SwitchPageTemplate("Main");
        }

        private System.Threading.Tasks.Task LoadMembersList()
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                List<User> members;
                bool success = ProjectManager.Instance.GetProjectMembers(Project.ProjectId, out members);

                Members = members;

                if (Members == null)
                {
                    MessageBox.Show("Unexpected error occured!");
                }
            });
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

        private void UpdateMembersContent()
        {
            var list = GetTemplateControl<ListBox>("MembersList");

            if (list == null)
            {
                return;
            }

            list.Items.Clear();

            foreach (var member in Members)
            {
                var item = new ListBoxItem()
                {
                    ContentTemplate = Resources["MemberListItem"] as DataTemplate,
                    Name = $"Member_{member.UserId}",
                    DataContext = member,
                    Content = member,
                    Margin = new(30, 0, 0, 0)
                };

                list.Items.Add(item);
            }
        }

        private async void UpdateMembersTick(object sender, EventArgs e)
        {
            await LoadMembersList();
            Dispatcher.Invoke(() => UpdateMembersContent());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Timer.Stop();

            switch (TemplateKey)
            {
                case "Main":
                    var taskListsPanel = GetTemplateControl<StackPanel>("TaskLists");

                    var taskList = new ContentControl()
                    {
                        ContentTemplate = Resources["TaskList"] as DataTemplate,
                        Content = TaskLists[0],
                        DataContext = TaskLists[0],
                        Height = 350
                    };

                    taskList.Loaded += (s, e) =>
                    {
                        var tasksPanel = VisualTreeTraversal.FindName(taskList, "Tasks") as StackPanel;

                        var task = new ContentControl()
                        {
                            ContentTemplate = Resources["Task"] as DataTemplate,
                            Content = Tasks[0],
                            DataContext = Tasks[0]
                        };

                        tasksPanel.Children.Add(task);
                    };

                    taskListsPanel.Children.Insert(0, taskList);

                    UpdateMainContent();
                    break;

                case "ProjectInfo":
                    UpdateProjectInfoContent();
                    break;

                case "ProjectMembers":
                    UpdateMembersContent();
                    Timer.Start();
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

        public void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = GetTemplateControl<TextBox>("SearchBoxText");
            var placeholder = GetTemplateControl<TextBlock>("SearchBoxPlaceholder");

            if (searchText.Text == "")
            {
                placeholder.Visibility = Visibility.Visible;
            }
            else
            {
                placeholder.Visibility = Visibility.Collapsed;
            }
        }

        public void InviteClick(object sender, RoutedEventArgs e)
        {
            var searchBox = GetTemplateControl<TextBox>("SearchBoxText").Text;
            var error = GetTemplateControl<TextBlock>("Error");

            if (searchBox == string.Empty)
            {
                error.Text = "Enter a valid email or username!";
                return;
            }

            User data = new()
            {
                Email = searchBox,
                Login = searchBox
            };

            bool success = UserManager.Instance.SendProjectInvite(data, Project);

            if (!success)
            {
                error.Text = "Could not send an invite!";
                return;
            }

            error.Foreground = Brushes.White;
            error.Text = "An invite has been sent!";
        }

        public void ManageClick(object sender, RoutedEventArgs e)
        {

        }

        public void KickClick(object sender, RoutedEventArgs e)
        {
            var panel = GetTemplateControl<Border>("KickPanel");
            var button = sender as Button;
            var userId = long.Parse(button.Tag.ToString());
            CurrentKickedUser = Members.Where(x => x.UserId == userId).FirstOrDefault();

            panel.Visibility = Visibility.Visible;
        }

        public void CancelKickClick(object sender, RoutedEventArgs e)
        {
            var panel = GetTemplateControl<Border>("KickPanel");
            panel.Visibility = Visibility.Collapsed;
        }

        public async void ConfirmKickClick(object sender, RoutedEventArgs e)
        {
            var panel = GetTemplateControl<Border>("KickPanel");

            ProjectManager.Instance.RemoveProjectMember(Project, CurrentKickedUser);
            CurrentKickedUser = null;
            await LoadMembersList();
            UpdateMembersContent();
            panel.Visibility = Visibility.Collapsed;
        }

        public void ClearCreateTaskListClick(object sender, RoutedEventArgs e)
        {
            var name = GetTemplateControl<TextBox>("CreateTaskListName");
            var description = GetTemplateControl<TextBox>("CreateTaskListDescription");

            name.Text = string.Empty;
            description.Text = string.Empty;
        }

        public void CreateTaskListClick(object sender, RoutedEventArgs e)
        {
            var name = GetTemplateControl<TextBox>("CreateTaskListName").Text;
            var description = GetTemplateControl<TextBox>("CreateTaskListDescription").Text;
            var errorPopup = GetTemplateControl<Border>("ErrorPopup");
            var errorText = GetTemplateControl<TextBlock>("Error");

            if (name == string.Empty || description == string.Empty)
            {
                errorText.Text = "You have to fill in every field!";
                errorPopup.Visibility = Visibility.Visible;
                return;
            }

            TaskList data = new();
            data.Name = name;
            data.Description = description;

            bool success = TaskListManager.Instance.CreateTaskList(data);

            if (success)
            {
                return;
            }

            errorText.Text = "There was an unexpected error!";
            errorPopup.Visibility = Visibility.Visible;
        }

        public void CloseErrorPopup(object sender, RoutedEventArgs e)
        {
            var errorPopup = GetTemplateControl<Border>("ErrorPopup");
            errorPopup.Visibility = Visibility.Collapsed;
        }

        public void CreateTaskNameTextChanged(object sender, TextChangedEventArgs e)
        {
            var name = sender as TextBox;
            var placeholder = name.Parent.FindVisualChildren<TextBlock>().ToList()[0];

            if (name.Text != "")
            {
                placeholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                placeholder.Visibility = Visibility.Visible;
            }
        }

        public void CreateTaskDescriptionTextChanged(object sender, TextChangedEventArgs e)
        {
            var description = sender as TextBox;
            var placeholder = description.Parent.FindVisualChildren<TextBlock>().ToList()[0];

            if (description.Text != "")
            {
                placeholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                placeholder.Visibility = Visibility.Visible;
            }
        }

        public void CreateTaskDeadlineTextChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var deadline = sender as DateTimePicker;
            var placeholder = deadline.Parent.FindVisualChildren<TextBlock>().ToList()[0];

            if (e.NewValue != null)
            {
                placeholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                placeholder.Visibility = Visibility.Visible;
            }
        }

        public void ClearCreateTaskClick(object sender, RoutedEventArgs e)
        {
            var buttonGrid = (sender as Button).Parent as Grid;
            var mainGrid = buttonGrid.Parent as Grid;

            var innerGrids = mainGrid.FindVisualChildren<Grid>().ToList();

            var name = innerGrids.Where(x => x.Name == "NameGrid").FirstOrDefault().FindVisualChildren<TextBox>().ToList()[0];
            var description = innerGrids.Where(x => x.Name == "DescriptionGrid").FirstOrDefault().FindVisualChildren<TextBox>().ToList()[0];
            var deadline = innerGrids.Where(x => x.Name == "DeadlineGrid").FirstOrDefault().FindVisualChildren<DateTimePicker>().ToList()[0];

            name.Text = string.Empty;
            description.Text = string.Empty;
            deadline.Value = null;
        }

        public void CreateTaskClick(object sender, RoutedEventArgs e)
        {
            var buttonGrid = (sender as Button).Parent as Grid;
            var mainGrid = buttonGrid.Parent as Grid;

            var innerGrids = mainGrid.FindVisualChildren<Grid>().ToList();

            var name = innerGrids.Where(x => x.Name == "NameGrid").FirstOrDefault().FindVisualChildren<TextBox>().ToList()[0].Text;
            var description = innerGrids.Where(x => x.Name == "DescriptionGrid").FirstOrDefault().FindVisualChildren<TextBox>().ToList()[0].Text;
            var deadline = innerGrids.Where(x => x.Name == "DeadlineGrid").FirstOrDefault().FindVisualChildren<DateTimePicker>().ToList()[0].Value;
            var errorPopup = GetTemplateControl<Border>("ErrorPopup");
            var errorText = GetTemplateControl<TextBlock>("Error");

            if (name == string.Empty || description == string.Empty || deadline == null)
            {
                errorPopup.Visibility = Visibility.Visible;
                errorText.Text = "You have to fill in every field!";
                return;
            }

            Task data = new()
            {
                Name = name,
                Description = description,
                Deadline = (DateTime)deadline
            };

            bool success = TaskManager.Instance.CreateTask(data);

            if (success)
            {
                return;
            }

            errorPopup.Visibility = Visibility.Visible;
            errorText.Text = "There was an unexpected error!";
        }
        
    }
}
