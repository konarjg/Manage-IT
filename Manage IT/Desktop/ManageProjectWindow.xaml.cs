using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        public List<TaskList> TaskLists { get; private set; }
        public UserPermissions Permissions { get; private set; }

        private string TemplateKey { get; set; }
        private User CurrentKickedUser { get; set; }
        private UserPermissions CurrentManagedPermissions { get; set; }

        private DispatcherTimer TimerMembers;

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

            UserPermissions permissions;

            if (!UserManager.Instance.GetCurrentUserPermissions(Project.ProjectId, out permissions))
            {
                MessageBox.Show("Unexpected error occured!");
                return;
            }

            Permissions = permissions;

            TimerMembers = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            TimerMembers.Tick += UpdateMembersTick;

            LoadMembersList();
            LoadTaskLists();
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

        private void LoadTaskLists()
        {
            List<TaskList> taskLists;
            bool success = TaskListManager.Instance.GetAllTaskLists(Project.ProjectId, out taskLists);

            TaskLists = taskLists;

            if (TaskLists == null)
            {
                MessageBox.Show("Unexpected error occured!");
            }
        }

        private void UpdateTopNavContent()
        {
            var edit = GetTemplateControl<Button>("Edit");
            var meeting = GetTemplateControl<Button>("Meeting");
            var delete = GetTemplateControl<Button>("Delete");

            if (edit == null || meeting == null || delete == null)
            {
                return;
            }

            if (Permissions.Editing)
            {
                return;
            }

            edit.Visibility = Visibility.Collapsed;
            meeting.Visibility = Visibility.Collapsed;
            delete.Visibility = Visibility.Collapsed;
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
            var searchBox = GetTemplateControl<Grid>("SearchBox");
            var invite = GetTemplateControl<Button>("Invite");

            if (list == null || searchBox == null || invite == null)
            {
                return;
            }

            if (!Permissions.InvitingMembers)
            {
                searchBox.Visibility = Visibility.Collapsed;
                invite.Visibility = Visibility.Collapsed;
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

                item.Loaded += (s, e) =>
                {
                    var panel = item.FindVisualChildren<StackPanel>().FirstOrDefault();

                    if (panel == null)
                    {
                        return;

                    }
                    var buttons = panel.FindVisualChildren<Button>();

                    if (buttons == null)
                    {
                        return;
                    }

                    var kick = buttons.Where(x => x.Name == "Kick").FirstOrDefault();
                    var manage = buttons.Where(x => x.Name == "Manage").FirstOrDefault();

                    if (kick == null || manage == null)
                    {
                        return;
                    }

                    if (!Permissions.Editing)
                    {
                        manage.Visibility = Visibility.Collapsed;
                    }

                    if (!Permissions.KickingMembers)
                    {
                        kick.Visibility = Visibility.Collapsed;
                    }
                };

                list.Items.Add(item);
            }
        }

        private void UpdateTaskListsContent()
        {
            var taskLists = GetTemplateControl<StackPanel>("TaskLists");
            var createTaskList = GetTemplateControl<ContentControl>("CreateTaskList");
            taskLists.Children.Clear();

            if (taskLists == null || TaskLists == null)
            {
                return;
            }

            if (!Permissions.Editing)
            {
                createTaskList.Visibility = Visibility.Collapsed;
            }

            foreach (var taskList in TaskLists)
            {
                var item = new ContentControl()
                {
                    Height = 350,
                    ContentTemplate = Resources["TaskList"] as DataTemplate,
                    DataContext = taskList,
                    Content = taskList
                };

                item.Loaded += (s, e) =>
                {
                    List<Task> tasks;

                    bool success = TaskManager.Instance.GetAllTasks(taskList.TaskListId, out tasks);

                    if (!success)
                    {
                        return;
                    }

                    var taskPanel = VisualTreeTraversal.FindName(item, "Tasks") as StackPanel;
                    var createTask = taskPanel.Children[0];

                    if (!Permissions.Editing)
                    {
                        createTask.Visibility = Visibility.Collapsed;
                    }

                    taskPanel.Children.Clear();

                    foreach (var task in tasks)
                    {
                        var taskItem = new ContentControl()
                        {
                            Width = 450,
                            Height = 200,
                            ContentTemplate = Resources["Task"] as DataTemplate,
                            DataContext = task,
                            Content = task
                        };

                        taskPanel.Children.Add(taskItem);
                    }

                    taskPanel.Children.Add(createTask);
                };

                taskLists.Children.Add(item);
            }

            taskLists.Children.Add(createTaskList);
        }

        private async void UpdateMembersTick(object sender, EventArgs e)
        {
            await LoadMembersList();
            await Dispatcher.InvokeAsync(() => UpdateMembersContent());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TimerMembers.Stop();
            UpdateTopNavContent();

            switch (TemplateKey)
            {
                case "Main":
                    LoadTaskLists();
                    UpdateMainContent();
                    UpdateTaskListsContent();
                    break;

                case "ProjectInfo":
                    UpdateProjectInfoContent();
                    break;

                case "ProjectMembers":
                    UpdateMembersContent();
                    TimerMembers.Start();
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
            var panel = GetTemplateControl<Border>("CreateMeetingPopup");
            panel.Visibility = Visibility.Visible;
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
            var panel = GetTemplateControl<Border>("ManageMemberPopup");
            var error = GetTemplateControl<TextBlock>("ManageMemberPopupError");

            var username = GetTemplateControl<TextBlock>("Username");
            var email = GetTemplateControl<TextBlock>("Email");
            var editing = GetTemplateControl<ToggleButton>("Editing");
            var inviting = GetTemplateControl<ToggleButton>("Inviting");
            var kicking = GetTemplateControl<ToggleButton>("Kicking");
            

            var userId = long.Parse((sender as Button).Tag.ToString());
            var user = Members.Where(x => x.UserId == userId).FirstOrDefault();
            UserPermissions permissions;

            bool success = UserManager.Instance.GetUserPermissions(userId, Project.ProjectId, out permissions);

            if (!success)
            {
                error.Text = "There was an unexpected error!";
                return;
            }

            username.Text = user.Login;
            email.Text = user.Email;
            editing.IsChecked = permissions.Editing;
            inviting.IsChecked = permissions.InvitingMembers;
            kicking.IsChecked = permissions.KickingMembers;
            CurrentManagedPermissions = permissions;

            panel.Visibility = Visibility.Visible;
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
            data.ProjectId = Project.ProjectId;

            bool success = TaskListManager.Instance.CreateTaskList(data);

            if (success)
            {
                LoadTaskLists();
                UpdateTaskListsContent();
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

            var border = mainGrid.Parent as Border;
            var taskList = border.Parent as ContentControl;
            var taskListId = long.Parse(taskList.Tag.ToString());

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
                Deadline = (DateTime)deadline,
                TaskListId = taskListId
            };

            bool success = TaskManager.Instance.CreateTask(data);

            if (success)
            {
                LoadTaskLists();
                UpdateTaskListsContent();
                return;
            }

            errorPopup.Visibility = Visibility.Visible;
            errorText.Text = "There was an unexpected error!";
        }

        public void ManageTaskList(object sender, RoutedEventArgs e)
        {

        }

        public void ManageTask(object sender, RoutedEventArgs e)
        {

        }

        public void CloseManageMember(object sender, RoutedEventArgs e)
        {
            var panel = GetTemplateControl<Border>("ManageMemberPopup");
            panel.Visibility = Visibility.Collapsed;
        }
        
        public void CancelMemberChanges(object sender, RoutedEventArgs e)
        {
            var editing = GetTemplateControl<ToggleButton>("Editing");
            var inviting = GetTemplateControl<ToggleButton>("Inviting");
            var kicking = GetTemplateControl<ToggleButton>("Kicking");

            editing.IsChecked = CurrentManagedPermissions.Editing;
            inviting.IsChecked = CurrentManagedPermissions.InvitingMembers;
            kicking.IsChecked = CurrentManagedPermissions.KickingMembers;
        }

        public void ConfirmMemberChanges(object sender, RoutedEventArgs e)
        {
            var editing = GetTemplateControl<ToggleButton>("Editing");
            var inviting = GetTemplateControl<ToggleButton>("Inviting");
            var kicking = GetTemplateControl<ToggleButton>("Kicking");
            var error = GetTemplateControl<TextBlock>("ManageMemberPopupError");

            CurrentManagedPermissions.Editing = editing.IsChecked != null ? (bool)editing.IsChecked : false;
            CurrentManagedPermissions.InvitingMembers = inviting.IsChecked != null ? (bool)inviting.IsChecked : false;
            CurrentManagedPermissions.KickingMembers = kicking.IsChecked != null ? (bool)kicking.IsChecked : false;

            bool success = UserManager.Instance.UpdateUserPermissions(CurrentManagedPermissions);

            if (!success)
            {
                error.Text = "Could not change permissions!";
                return;
            }

            error.Foreground = Brushes.White;
            error.Text = "Permissions changed!";
        }

        public void CloseMeeting(object sender, RoutedEventArgs e)
        {
            var panel = GetTemplateControl<Border>("CreateMeetingPopup");
            panel.Visibility = Visibility.Collapsed;
        }

        public void CreateMeeting(object sender, RoutedEventArgs e)
        {
            var panel = GetTemplateControl<Border>("CreateMeetingPopup");
            var error = GetTemplateControl<TextBlock>("MeetingError");
            var title = GetTemplateControl<TextBox>("MeetingTitle").Text;
            var description = GetTemplateControl<TextBox>("MeetingDescription").Text;
            var date = GetTemplateControl<DateTimePicker>("MeetingDate").Value;

            if (title == "" || description == "" || date == null)
            {
                error.Text = "You have to fill in every field!";
                return;
            }

            var data = new Meeting()
            {
                Title = title,
                Description = description,
                Date = (DateTime)date,
                ProjectId = Project.ProjectId
            };

            bool success = MeetingManager.Instance.CreateMeeting(data);

            if (!success)
            {
                error.Text = "You have to fill in every field!";
                return;
            }

            panel.Visibility = Visibility.Collapsed;
        }

    }
}
