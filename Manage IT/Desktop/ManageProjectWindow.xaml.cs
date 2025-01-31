using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.Toolkit;
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
                DependencyObject MyChild = VisualTreeHelper.GetChild(parent, i);
                if (MyChild is FrameworkElement && ((FrameworkElement)MyChild).Name == controlName)
                    return MyChild;

                DependencyObject FindResult = FindName(MyChild, controlName);
                if (FindResult != null)
                    return FindResult;
            }

            return null;
        }
    }

    public class MemberModel
    {
        public string Login { get; set; }
        public bool Assigned { get; set; }
        public User User { get; set; }

        public MemberModel(User user, List<User> taskMembers)
        {
            User = user;
            Assigned = taskMembers.FirstOrDefault(x => x.UserId == user.UserId) != null;
            Login = user.Login;
        }
    }

    public class TaskListModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskList TaskList { get; set; }
        public bool IsManager { get; set; }
        public Visibility ManagerVisible { get; set; }
        public string CloseText { get; set; }

        public TaskListModel(TaskList list, UserPermissions permissions)
        {
            TaskList = list;
            Name = list.Name;
            Description = list.Description;
            IsManager = permissions.Editing;
            ManagerVisible = IsManager ? Visibility.Visible : Visibility.Collapsed;
            CloseText = "Close";

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(IsManager));
            OnPropertyChanged(nameof(ManagerVisible));
            OnPropertyChanged(nameof(CloseText));
        }

        public bool Save()
        {
            if (Name == "" || Description == "")
            {
                return false;
            }

            TaskList.Name = Name;
            TaskList.Description = Description;
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class TaskModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public ObservableCollection<TaskList> TaskLists { get; set; }
        public ObservableCollection<MemberModel> Members { get; set; }
        public TaskList SelectedTaskList { get; set; }
        public ObservableCollection<MemberModel> SelectedMembers { get; set; }
        public Task Task { get; set; }
        public List<User> TaskMembers { get; set; }
        public bool IsManager { get; set; }
        public Visibility ManagerVisible { get; set; }
        public Visibility UserVisible { get; set; }
        public Visibility EditVisible { get; set; }
        public string CloseText { get; set; }
        public bool ReviewEnabled { get; set; }
        public bool HandInEnabled { get; set; }

        public TaskModel(Task task, List<TaskList> taskLists, List<User> members, List<User> taskMembers, Project project, UserPermissions permissions)
        {
            Task = task;
            Name = task.Name;
            Description = task.Description;
            Deadline = task.Deadline;
            IsManager = permissions.Editing;
            EditVisible = IsManager ? Visibility.Visible : Visibility.Collapsed;
            ManagerVisible = project.ManagerId == UserManager.Instance.CurrentSessionUser.UserId ? Visibility.Visible : Visibility.Collapsed;
            UserVisible = project.ManagerId != UserManager.Instance.CurrentSessionUser.UserId ? Visibility.Visible : Visibility.Hidden;
            Members = new();
            TaskLists = new();
            TaskMembers = taskMembers;
            CloseText = "Close";

            ReviewEnabled = task.HandedIn;
            HandInEnabled = !task.HandedIn;

            SelectedTaskList = taskLists.Where(x => x.TaskListId == task.TaskListId).FirstOrDefault();

            foreach (TaskList taskList in taskLists)
            {
                TaskLists.Add(taskList);
            }

            SelectedMembers = new();

            foreach (User member in members)
            {
                MemberModel item = new MemberModel(member, taskMembers);
                Members.Add(item);

                if (item.Assigned)
                {
                    SelectedMembers.Add(item);
                }
            }

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Deadline));
            OnPropertyChanged(nameof(IsManager));
            OnPropertyChanged(nameof(ManagerVisible));
            OnPropertyChanged(nameof(UserVisible));
            OnPropertyChanged(nameof(TaskLists));
            OnPropertyChanged(nameof(Members));
            OnPropertyChanged(nameof(CloseText));
            OnPropertyChanged(nameof(SelectedTaskList));
            OnPropertyChanged(nameof(SelectedMembers));
            OnPropertyChanged(nameof(ReviewEnabled));
            OnPropertyChanged(nameof(HandInEnabled));
        }

        public void SaveHandIn()
        {
            ReviewEnabled = Task.HandedIn;
            HandInEnabled = !Task.HandedIn;
            OnPropertyChanged(nameof(ReviewEnabled));
            OnPropertyChanged(nameof(HandInEnabled));
        }

        public void SaveReview()
        {
            ReviewEnabled = Task.HandedIn;
            HandInEnabled = !Task.HandedIn;
            OnPropertyChanged(nameof(ReviewEnabled));
            OnPropertyChanged(nameof(HandInEnabled));
        }

        public bool Save()
        {
            if (Name == "" || Description == "" || Deadline == null || SelectedTaskList == null)
            {
                return false;
            }

            Task.Name = Name;
            Task.Description = Description;
            Task.Deadline = (DateTime)Deadline;
            Task.TaskListId = SelectedTaskList.TaskListId;
            TaskMembers.Clear();

            foreach (MemberModel member in SelectedMembers)
            {
                TaskMembers.Add(member.User);
            }

            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
            Button edit = GetTemplateControl<Button>("Edit");
            Button meeting = GetTemplateControl<Button>("Meeting");
            Button delete = GetTemplateControl<Button>("Delete");

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
            TextBlock projectName = GetTemplateControl<TextBlock>("ProjectName");

            if (projectName == null)
            {
                return;
            }

            projectName.Text = Project.Name;
        }

        private void UpdateProjectInfoContent()
        {
            TextBox projectName = GetTemplateControl<TextBox>("ProjectName");
            TextBox description = GetTemplateControl<TextBox>("Description");

            if (projectName == null || description == null)
            {
                return;
            }

            projectName.Text = Project.Name;
            description.Text = Project.Description;
        }

        private void UpdateEditContent()
        {
            TextBox projectName = GetTemplateControl<TextBox>("ProjectName");
            TextBox description = GetTemplateControl<TextBox>("Description");

            if (projectName == null || description == null)
            {
                return;
            }

            projectName.Text = Project.Name;
            description.Text = Project.Description;
        }

        private void UpdateDeleteContent()
        {
            TextBlock placeholder = GetTemplateControl<TextBlock>("ProjectNamePlaceholder");

            if (placeholder == null)
            {
                return;
            }

            placeholder.Text = Project.Name;
        }

        private void UpdateMembersContent()
        {
            ListBox list = GetTemplateControl<ListBox>("MembersList");
            Grid searchBox = GetTemplateControl<Grid>("SearchBox");
            Button invite = GetTemplateControl<Button>("Invite");

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

            foreach (User member in Members)
            {
                ListBoxItem item = new ListBoxItem()
                {
                    ContentTemplate = Resources["MemberListItem"] as DataTemplate,
                    Name = $"Member_{member.UserId}",
                    DataContext = member,
                    Content = member,
                    Margin = new(30, 0, 0, 0)
                };

                item.Loaded += (s, e) =>
                {
                    StackPanel? panel = item.FindVisualChildren<StackPanel>().FirstOrDefault();

                    if (panel == null)
                    {
                        return;

                    }
                    IEnumerable<Button> buttons = panel.FindVisualChildren<Button>();

                    if (buttons == null)
                    {
                        return;
                    }

                    Button? kick = buttons.Where(x => x.Name == "Kick").FirstOrDefault();
                    Button? manage = buttons.Where(x => x.Name == "Manage").FirstOrDefault();

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
            StackPanel taskLists = GetTemplateControl<StackPanel>("TaskLists");
            ContentControl createTaskList = GetTemplateControl<ContentControl>("CreateTaskList");
            taskLists.Children.Clear();

            if (taskLists == null || TaskLists == null)
            {
                return;
            }

            if (!Permissions.Editing)
            {
                createTaskList.Visibility = Visibility.Collapsed;
            }

            foreach (TaskList taskList in TaskLists)
            {
                ContentControl item = new ContentControl()
                {
                    Height = 350,
                    ContentTemplate = Resources["TaskList"] as DataTemplate,
                    DataContext = taskList,
                    Content = taskList
                };

                item.Loaded += (s, e) =>
                {
                    List<Task> tasks;
                    bool success;

                    if (Project.ManagerId == UserManager.Instance.CurrentSessionUser.UserId)
                    {
                        success = TaskManager.Instance.GetAllTasks(taskList.TaskListId, out tasks);
                    }
                    else
                    {
                        success = TaskManager.Instance.GetAllAssignedTasks(taskList.TaskListId, UserManager.Instance.CurrentSessionUser.UserId, out tasks);
                    }

                    if (!success)
                    {
                        return;
                    }

                    StackPanel? taskPanel = VisualTreeTraversal.FindName(item, "Tasks") as StackPanel;
                    UIElement createTask = taskPanel.Children[0];

                    if (!Permissions.Editing)
                    {
                        createTask.Visibility = Visibility.Collapsed;
                    }

                    taskPanel.Children.Clear();

                    foreach (Task task in tasks)
                    {
                        ContentControl taskItem = new ContentControl()
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

            if (UserManager.Instance.CurrentSessionUser.Admin)
            {
                GetTemplateControl<Button>("AdminPanel").Visibility = Visibility.Visible;
            }

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

        public void AdminPanelClick(object sender, RoutedEventArgs e)
        {
            AdminPanelWindow window = new AdminPanelWindow(Project, true);
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
            Border panel = GetTemplateControl<Border>("CreateMeetingPopup");
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
            string projectName = GetTemplateControl<TextBox>("ProjectName").Text;
            string description = GetTemplateControl<TextBox>("Description").Text;

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
            string projectName = GetTemplateControl<TextBox>("ProjectName").Text;

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
            TextBox projectName = GetTemplateControl<TextBox>("ProjectName");
            TextBlock placeholder = GetTemplateControl<TextBlock>("ProjectNamePlaceholder");

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
            TextBox searchText = GetTemplateControl<TextBox>("SearchBoxText");
            TextBlock placeholder = GetTemplateControl<TextBlock>("SearchBoxPlaceholder");

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
            string searchBox = GetTemplateControl<TextBox>("SearchBoxText").Text;
            TextBlock error = GetTemplateControl<TextBlock>("Error");

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
            Border panel = GetTemplateControl<Border>("ManageMemberPopup");
            TextBlock error = GetTemplateControl<TextBlock>("ManageMemberPopupError");

            TextBlock username = GetTemplateControl<TextBlock>("Username");
            TextBlock email = GetTemplateControl<TextBlock>("Email");
            ToggleButton editing = GetTemplateControl<ToggleButton>("Editing");
            ToggleButton inviting = GetTemplateControl<ToggleButton>("Inviting");
            ToggleButton kicking = GetTemplateControl<ToggleButton>("Kicking");


            long userId = long.Parse((sender as Button).Tag.ToString());
            User? user = Members.Where(x => x.UserId == userId).FirstOrDefault();
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
            Border panel = GetTemplateControl<Border>("KickPanel");
            Button? button = sender as Button;
            long userId = long.Parse(button.Tag.ToString());
            CurrentKickedUser = Members.Where(x => x.UserId == userId).FirstOrDefault();

            panel.Visibility = Visibility.Visible;
        }

        public void CancelKickClick(object sender, RoutedEventArgs e)
        {
            Border panel = GetTemplateControl<Border>("KickPanel");
            panel.Visibility = Visibility.Collapsed;
        }

        public async void ConfirmKickClick(object sender, RoutedEventArgs e)
        {
            Border panel = GetTemplateControl<Border>("KickPanel");

            ProjectManager.Instance.RemoveProjectMember(Project, CurrentKickedUser);
            CurrentKickedUser = null;
            await LoadMembersList();
            UpdateMembersContent();
            panel.Visibility = Visibility.Collapsed;
        }

        public void ClearCreateTaskListClick(object sender, RoutedEventArgs e)
        {
            TextBox name = GetTemplateControl<TextBox>("CreateTaskListName");
            TextBox description = GetTemplateControl<TextBox>("CreateTaskListDescription");

            name.Text = string.Empty;
            description.Text = string.Empty;
        }

        public void CreateTaskListClick(object sender, RoutedEventArgs e)
        {
            string name = GetTemplateControl<TextBox>("CreateTaskListName").Text;
            string description = GetTemplateControl<TextBox>("CreateTaskListDescription").Text;
            Border errorPopup = GetTemplateControl<Border>("ErrorPopup");
            TextBlock errorText = GetTemplateControl<TextBlock>("Error");

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
            Border errorPopup = GetTemplateControl<Border>("ErrorPopup");
            errorPopup.Visibility = Visibility.Collapsed;
        }

        public void CreateTaskNameTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? name = sender as TextBox;
            TextBlock placeholder = name.Parent.FindVisualChildren<TextBlock>().ToList()[0];

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
            TextBox? description = sender as TextBox;
            TextBlock placeholder = description.Parent.FindVisualChildren<TextBlock>().ToList()[0];

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
            DateTimePicker? deadline = sender as DateTimePicker;
            TextBlock placeholder = deadline.Parent.FindVisualChildren<TextBlock>().ToList()[0];

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
            Grid? buttonGrid = (sender as Button).Parent as Grid;
            Grid? mainGrid = buttonGrid.Parent as Grid;

            List<Grid> innerGrids = mainGrid.FindVisualChildren<Grid>().ToList();

            TextBox name = innerGrids.Where(x => x.Name == "NameGrid").FirstOrDefault().FindVisualChildren<TextBox>().ToList()[0];
            TextBox description = innerGrids.Where(x => x.Name == "DescriptionGrid").FirstOrDefault().FindVisualChildren<TextBox>().ToList()[0];
            DateTimePicker deadline = innerGrids.Where(x => x.Name == "DeadlineGrid").FirstOrDefault().FindVisualChildren<DateTimePicker>().ToList()[0];

            name.Text = string.Empty;
            description.Text = string.Empty;
            deadline.Value = null;
        }

        public void CreateTaskClick(object sender, RoutedEventArgs e)
        {
            Grid? buttonGrid = (sender as Button).Parent as Grid;
            Grid? mainGrid = buttonGrid.Parent as Grid;

            Border? border = mainGrid.Parent as Border;
            ContentControl? taskList = border.Parent as ContentControl;
            long taskListId = long.Parse(taskList.Tag.ToString());

            List<Grid> innerGrids = mainGrid.FindVisualChildren<Grid>().ToList();

            string name = innerGrids.Where(x => x.Name == "NameGrid").FirstOrDefault().FindVisualChildren<TextBox>().ToList()[0].Text;
            string description = innerGrids.Where(x => x.Name == "DescriptionGrid").FirstOrDefault().FindVisualChildren<TextBox>().ToList()[0].Text;
            DateTime? deadline = innerGrids.Where(x => x.Name == "DeadlineGrid").FirstOrDefault().FindVisualChildren<DateTimePicker>().ToList()[0].Value;
            Border errorPopup = GetTemplateControl<Border>("ErrorPopup");
            TextBlock errorText = GetTemplateControl<TextBlock>("Error");

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

        public void CloseManageMember(object sender, RoutedEventArgs e)
        {
            Border panel = GetTemplateControl<Border>("ManageMemberPopup");
            panel.Visibility = Visibility.Collapsed;
        }

        public void CancelMemberChanges(object sender, RoutedEventArgs e)
        {
            ToggleButton editing = GetTemplateControl<ToggleButton>("Editing");
            ToggleButton inviting = GetTemplateControl<ToggleButton>("Inviting");
            ToggleButton kicking = GetTemplateControl<ToggleButton>("Kicking");

            editing.IsChecked = CurrentManagedPermissions.Editing;
            inviting.IsChecked = CurrentManagedPermissions.InvitingMembers;
            kicking.IsChecked = CurrentManagedPermissions.KickingMembers;
        }

        public void ConfirmMemberChanges(object sender, RoutedEventArgs e)
        {
            ToggleButton editing = GetTemplateControl<ToggleButton>("Editing");
            ToggleButton inviting = GetTemplateControl<ToggleButton>("Inviting");
            ToggleButton kicking = GetTemplateControl<ToggleButton>("Kicking");
            TextBlock error = GetTemplateControl<TextBlock>("ManageMemberPopupError");

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
            Border panel = GetTemplateControl<Border>("CreateMeetingPopup");
            panel.Visibility = Visibility.Collapsed;
            GetTemplateControl<Button>("BackButton").Visibility = Visibility.Visible;
        }

        public void CreateMeeting(object sender, RoutedEventArgs e)
        {
            GetTemplateControl<Button>("BackButton").Visibility = Visibility.Collapsed;
            Border panel = GetTemplateControl<Border>("CreateMeetingPopup");
            TextBlock error = GetTemplateControl<TextBlock>("MeetingError");
            string title = GetTemplateControl<TextBox>("MeetingTitle").Text;
            string description = GetTemplateControl<TextBox>("MeetingDescription").Text;
            DateTime? date = GetTemplateControl<DateTimePicker>("MeetingDate").Value;

            if (title == "" || description == "" || date == null)
            {
                error.Text = "You have to fill in every field!";
                return;
            }

            Meeting data = new Meeting()
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
            GetTemplateControl<Button>("BackButton").Visibility = Visibility.Visible;
        }

        public void ManageTaskList(object sender, RoutedEventArgs e)
        {
            GetTemplateControl<Button>("BackButton").Visibility = Visibility.Collapsed;
            Border popup = GetTemplateControl<Border>("ManageTaskListPopup");
            long? taskListId = (sender as Button).Tag as long?;
            TaskList taskList = TaskListManager.Instance.GetTaskList((long)taskListId);
            UserPermissions permissions = Permissions;

            popup.DataContext = new TaskListModel(taskList, permissions);
            popup.Visibility = Visibility.Visible;
        }

        public void ManageTask(object sender, RoutedEventArgs e)
        {
            GetTemplateControl<Button>("BackButton").Visibility = Visibility.Collapsed;
            Border popup = GetTemplateControl<Border>("ManageTaskPopup");
            long? taskId = (sender as Button).Tag as long?;
            Task task = TaskManager.Instance.GetTask((long)taskId);
            UserPermissions permissions = Permissions;

            List<User> members;
            List<User> taskMembers = TaskManager.Instance.GetMembers(task.TaskId);
            ProjectManager.Instance.GetProjectMembers(Project.ProjectId, out members);

            popup.DataContext = new TaskModel(task, TaskLists, members, taskMembers, Project, permissions);
            popup.Visibility = Visibility.Visible;
        }

        public void EditDeleteTaskList(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ManageTaskListPopup");
            TextBlock error = GetTemplateControl<TextBlock>("EditTaskListError");
            TaskListModel? model = popup.DataContext as TaskListModel;

            TaskList data = model.TaskList;

            bool success = TaskListManager.Instance.DeleteTaskList(data.TaskListId);

            if (success)
            {
                LoadTaskLists();

                Dispatcher.Invoke(() =>
                {
                    UpdateTaskListsContent();
                });

                error.Foreground = Brushes.White;
                error.Text = "Task list edited!";
                popup.Visibility = Visibility.Collapsed;
                return;
            }

            error.Text = "There was an unexpected error!";
        }

        public void EditCloseTaskList(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ManageTaskListPopup");
            popup.Visibility = Visibility.Collapsed;
            GetTemplateControl<Button>("BackButton").Visibility = Visibility.Visible;
        }

        public void EditConfirmTaskList(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ManageTaskListPopup");
            TextBlock error = GetTemplateControl<TextBlock>("EditTaskListError");
            TaskListModel? model = popup.DataContext as TaskListModel;
            bool saved = model.Save();

            if (!saved)
            {
                error.Text = "You have to fill in every field!";
                return;
            }

            TaskList data = model.TaskList;

            bool success = TaskListManager.Instance.UpdateTaskList(data);

            if (success)
            {
                LoadTaskLists();

                Dispatcher.Invoke(() =>
                {
                    UpdateTaskListsContent();
                });

                error.Foreground = Brushes.White;
                error.Text = "Task list edited!";
                return;
            }

            error.Text = "There was an unexpected error!";
        }

        public void EditReviewTask(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ReviewPopup");
            popup.Visibility = Visibility.Visible;
        }

        public void CancelReview(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ReviewPopup");
            popup.Visibility = Visibility.Collapsed;
        }

        public void RejectReview(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ReviewPopup");
            TaskModel? model = GetTemplateControl<Border>("ManageTaskPopup").DataContext as TaskModel;
            TextBlock error = GetTemplateControl<TextBlock>("EditTaskError");

            model.Task.Accepted = false;
            model.Task.HandedIn = false;
            bool success = TaskManager.Instance.UpdateTask(model.Task);

            if (!success)
            {
                popup.Visibility = Visibility.Collapsed;
                error.Text = "There was an unexpected error!";
                return;
            }

            model.SaveReview();
            popup.Visibility = Visibility.Collapsed;
            error.Foreground = Brushes.White;
            error.Text = "Task has been rejected!";

            LoadTaskLists();

            Dispatcher.Invoke(() =>
            {
                UpdateTaskListsContent();
            });
        }

        public void AcceptReview(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ReviewPopup");
            TaskModel? model = GetTemplateControl<Border>("ManageTaskPopup").DataContext as TaskModel;
            TextBlock error = GetTemplateControl<TextBlock>("EditTaskError");

            model.Task.Accepted = true;
            bool success = TaskManager.Instance.UpdateTask(model.Task);

            if (!success)
            {
                popup.Visibility = Visibility.Collapsed;
                error.Text = "There was an unexpected error!";
                return;
            }

            model.SaveReview();
            popup.Visibility = Visibility.Collapsed;
            error.Foreground = Brushes.White;
            error.Text = "Task has been accepted!";
            LoadTaskLists();

            Dispatcher.Invoke(() =>
            {
                UpdateTaskListsContent();
            });
        }

        public void EditHandInTask(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("HandInPopup");
            popup.Visibility = Visibility.Visible;
            GetTemplateControl<Button>("BackButton").Visibility = Visibility.Visible;
        }

        public void CancelHandIn(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("HandInPopup");
            popup.Visibility = Visibility.Collapsed;
        }

        public void ConfirmHandIn(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("HandInPopup");
            TaskModel? model = GetTemplateControl<Border>("ManageTaskPopup").DataContext as TaskModel;
            TextBlock error = GetTemplateControl<TextBlock>("EditTaskError");

            model.Task.HandedIn = true;
            model.Task.Accepted = null;
            bool success = TaskManager.Instance.UpdateTask(model.Task);

            if (!success)
            {
                popup.Visibility = Visibility.Collapsed;
                error.Text = "There was an unexpected error!";
                return;
            }

            model.SaveHandIn();
            popup.Visibility = Visibility.Collapsed;
            error.Foreground = Brushes.White;
            error.Text = "Task has been handed in!";

            LoadTaskLists();

            Dispatcher.Invoke(() =>
            {
                UpdateTaskListsContent();
            });
        }

        public void EditDeleteTask(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ManageTaskPopup");
            TextBlock error = GetTemplateControl<TextBlock>("EditTaskError");
            TaskModel? model = popup.DataContext as TaskModel;

            Task data = model.Task;

            bool success = TaskManager.Instance.DeleteTask(data.TaskId);

            if (success)
            {
                LoadTaskLists();

                Dispatcher.Invoke(() =>
                {
                    UpdateTaskListsContent();
                });

                popup.Visibility = Visibility.Collapsed;
                GetTemplateControl<Button>("BackButton").Visibility = Visibility.Visible;
                return;
            }

            error.Text = "There was an unexpected error!";
        }

        public void EditCloseTask(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ManageTaskPopup");
            popup.Visibility = Visibility.Collapsed;
            GetTemplateControl<Button>("BackButton").Visibility = Visibility.Visible;
        }

        public void EditConfirmTask(object sender, RoutedEventArgs e)
        {
            Border popup = GetTemplateControl<Border>("ManageTaskPopup");
            TextBlock error = GetTemplateControl<TextBlock>("EditTaskError");
            TaskModel? model = popup.DataContext as TaskModel;
            bool saved = model.Save();

            if (!saved)
            {
                error.Text = "You have to fill in every field and select a task list!";
                return;
            }

            Task data = model.Task;
            List<User> members = model.TaskMembers;

            bool success = TaskManager.Instance.UpdateTask(data) && TaskManager.Instance.AssignMembers(data.TaskId, members);

            if (success)
            {
                LoadTaskLists();

                Dispatcher.Invoke(() =>
                {
                    UpdateTaskListsContent();
                });

                error.Foreground = Brushes.White;
                error.Text = "Task edited!";
                return;
            }

            error.Text = "There was an unexpected error!";
        }
    }
}
