<<<<<<< HEAD
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
using System.Diagnostics.Metrics;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;
using Xceed.Wpf.Toolkit;



namespace Desktop
{
    public class AdminPanelViewModel
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /// <summary>
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {

        private Stack<ControlTemplate> templateHistory;
        Project project = new Project();
        bool backToProject = false;
        EFModeling.EntityProperties.DataAnnotations.Annotations.Task currentlyEditedTask;
        TaskList currentlyEditedTaskList;
        Meeting currentlyEditedMeeting;

        User pickedUser; //we will use it for editing specific user or tranferring rights to it.

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
                System.Windows.MessageBox.Show($"Template '{name}' not found in resources.");
            }

        }

        /* public void UsersList(object sender, RoutedEventArgs e)
         {

             string error;
             bool success = ProjectManager.GetProjectMembers(project.ProjectId, out List < User > members); // Assume GetProjectMembers sets the error message

             if (!success || !string.IsNullOrEmpty(error))
             {
                 MessageBox.Show(error);
                 return;
             }

             Panel membersListPanel = GetTemplateControl<Panel>("MembersList"); // Changing ScrollViewer to Panel

             foreach (var member in members)
             {
                 Button button = new();
                 button.Content = member.Name;
                 button.Name = $"Member{member.UserId}";
                 button.Click += MemberClick;

                 if (member.UserId == UserManager.Instance.CurrentSessionUser.UserId)
                 {
                     button.Style = (Style)FindResource("CurrentMemberButton");
                 }
                 else
                 {
                     button.Style = (Style)FindResource("MemberButton");
                 }

                 membersListPanel.Children.Insert(0, button); // Adding to Panel's children
             }
         }*/
        public void UsersListViewLoaded(object sender, RoutedEventArgs e)
        {
            List<User> members;
            string error = String.Empty;
            bool success = ProjectManager.Instance.GetProjectMembers(project.ProjectId, out members);

            if (!success || !string.IsNullOrEmpty(error))
            {
                return;
            }

            StackPanel membersListPanel = sender as StackPanel; // Cast sender to StackPanel
            if (membersListPanel == null)
            {
                return;
            }

            foreach (var member in members)
            {
                Button button = new Button();
                button.Content = $"id:{member.UserId} | {member.Login}";
                button.Name = $"Member{member.UserId}";
                //button.Click += MemberClick;
                button.Style = (Style)FindResource("MemberButton");

                membersListPanel.Children.Insert(0, button); // Adding to StackPanel's children
            }
        }
        public void MeetingsListViewLoaded(object sender, RoutedEventArgs e)
        {
            List<Meeting> meetings;
            string error = String.Empty;
            bool success = MeetingManager.Instance.GetMeetingsRelatedToProject(project.ProjectId, out meetings);

            if (!success || !string.IsNullOrEmpty(error))
            {
                return;
            }

            StackPanel membersListPanel = sender as StackPanel; // Cast sender to StackPanel
            if (membersListPanel == null)
            {
                
                return;
            }

            foreach (var meeting in meetings)
            {
                Button button = new Button();
                button.Content = $"id:{meeting.MeetingId} | {meeting.Title}";
                button.Name = $"Meeting{meeting.MeetingId}";
                button.Tag = meeting.MeetingId;
                button.Click += MeetingsEditMeetingClick;
                button.Style = (Style)FindResource("MemberButton");

                membersListPanel.Children.Insert(0, button); // Adding to StackPanel's children
            }
        }


        //basically for that manager info display
        public void PutProjectManagerUser(object sender, RoutedEventArgs e)
        {

            if (sender is TextBlock textBlock && project != null)
            {
                // Replace these with actual user and UID values
                bool success = UserManager.Instance.GetUser(project.ManagerId, out User user);

                textBlock.Text = $"Manager: {user.Login}";
            }

        }
        public void PutProjectManagerUserID(object sender, RoutedEventArgs e)
        {

            if (sender is TextBlock textBlock && project != null)
            {
                // Replace these with actual user and UID values
                bool success = UserManager.Instance.GetUser(project.ManagerId, out User user);


                textBlock.Text = $"UID: {user.UserId}";
            }

        }

        public void TasksViewLoaded(object sender, RoutedEventArgs e)
        {
            List<TaskList> taskLists;
            string error = String.Empty;
            bool success = TaskListManager.Instance.GetAllTaskLists(project.ProjectId, out taskLists);

            if (!success || !string.IsNullOrEmpty(error))
            {
                return;
            }

            StackPanel taskListsListPanel = sender as StackPanel; // Cast sender to StackPanel
            if (taskListsListPanel == null)
            {
                //MessageBox.Show("Sender is not a StackPanel");
                return;
            }

            foreach (var taskList in taskLists)
            {
                Button button = new();
                button.Content = $"id:{taskList.TaskListId} | {taskList.Name}";
                button.Name = $"Member{taskList.TaskListId}";
                button.Tag = taskList.TaskListId;
                button.Click += TasksEditTaskListClick;
                button.Style = (Style)FindResource("MemberButton");
                List<EFModeling.EntityProperties.DataAnnotations.Annotations.Task> tasks;
                bool successTasks = TaskManager.Instance.GetAllTasks(taskList.TaskListId, out tasks);
                taskListsListPanel.Children.Insert(0, button); // Adding to StackPanel's children
                if (successTasks)
                {
                    foreach (var task in tasks)
                    {
                        Button button2 = new Button();
                        button2.Content = $"id:{task.TaskId} | {task.Name}";
                        button2.Name = $"List{taskList.TaskListId}Task{task.TaskId}";
                        button2.Tag = task.TaskId;
                        button2.Style = (Style)FindResource("SubmemberButton");
                        button2.Click += TasksEditTaskClick;
                        taskListsListPanel.Children.Insert(1, button2);
                    }
                }

            }
            /*for (int i = 0; i < 10; i++)
            {
                Button button = new Button();
                button.Content = $"Task lis{i}";
                button.Name = $"List{i}";
                //button.Click += MemberClick;
                button.Style = (Style)FindResource("MemberButton");
                taskListsListPanel.Children.Insert(0, button); // Adding to StackPanel's children

                for (int j = 0; j < 5; j++)
                {
                    Button button2 = new Button();
                    button2.Content = $"Task {j}";
                    button2.Name = $"List{i}Task{j}";
                    button2.Style = (Style)FindResource("SubmemberButton");
                    taskListsListPanel.Children.Insert(1, button2); // Adding to StackPanel's children
                                                                    //button2.Click += MemberClick;
                }
            }*/
        }













        private void ProjectNameHeaderDispName(object sender, RoutedEventArgs e)
        {
            TextBlock header = GetTemplateControl<TextBlock>("ProjectNameHeader");
            //header.Text = project.Name;
            header.Text = project.Name;
        }
        private void ReplaceProjectNameInText(object sender, RoutedEventArgs e) {
            string textToReplace = "(project.name)";
            string projectName = project.Name;
            if (sender is TextBlock textBlock) {
                textBlock.Text = textBlock.Text.Replace(textToReplace, projectName);
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
                if (backToProject)
                {
                    var window = new ManageProjectWindow(project);
                    window.Activate();
                    window.Visibility = Visibility.Visible;
                }
                else
                {
                    var window = new AdminPanelWindowMain();
                    window.Activate();
                    window.Visibility = Visibility.Visible;
                }


                Close();
            }

        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void SendInvitationClick(object sender, RoutedEventArgs e)
        {
            TextBlock Error = GetTemplateControl<TextBlock>("Error");

            var credential = GetTemplateControl<TextBox>("UserToInvite").Text;
            User data = new();
            data.Login = credential;
            data.Email = credential;
            bool successUE = UserManager.Instance.UserExists(data, out User user);
            Error.Text = credential;
            if (successUE)
            {
                UserManager.Instance.SendProjectInvite(user, project);
                Error.Foreground = Brushes.White;
                Error.Text = "User has been invited";
            }
            else
            {
                Error.Foreground = Brushes.Red;
                Error.Text = "User hasn't been found";
            }

        }

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
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject { if (depObj != null) { for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) { DependencyObject child = VisualTreeHelper.GetChild(depObj, i); if (child != null && child is T) { yield return (T)child; } foreach (T childOfChild in FindVisualChildren<T>(child)) { yield return childOfChild; } } } }


        public AdminPanelWindow(Project project, bool backToProject)
        {
            InitializeComponent();
            //add logic to lead picked project name

            templateHistory = new Stack<ControlTemplate>();
            DataContext = new AdminPanelViewModel();
            this.project = project;
            this.backToProject = backToProject;
        }


        public void BackClick(object sender, RoutedEventArgs e)
        {
            //prolly if in main adm panel window then do this.close()
            SwitchBackToPreviousTemplate();
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

        public void SupportClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Support");
        }
        public void TasksClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Tasks");
            TasksViewLoaded(sender, e);
        }



        public void SecurityClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Security");
        }

        public void AuditLogClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("AuditLog");
        }

        public void InvitesClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Invites");


        }


        // Example methods for click events
        private void DeclineInviteButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int userId = (int)button.Tag;
            User user;
            UserManager.Instance.GetUser(userId, out user);
            ProjectManager.Instance.RemoveProjectMember(project, user);


            SwitchPageTemplate("Invites");
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            Error.Foreground = Brushes.Red;
            Error.Text = "Invite has been declined";

            // Your decline logic here, using the userId
        }

        private void AcceptInviteButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int userId = (int)button.Tag;
            User user;
            UserManager.Instance.GetUser(userId, out user);
            bool success = ProjectManager.Instance.AcceptInvite(project, user);
            if (success)
            {
                SwitchPageTemplate("Invites");
                TextBlock Error = GetTemplateControl<TextBlock>("Error");
                Error.Foreground = Brushes.White;
                Error.Text = "Invite has been accepted";
            }
            else
            {
                TextBlock Error = GetTemplateControl<TextBlock>("Error");
                Error.Foreground = Brushes.Red;
                Error.Text = "An error has occured";
            }
            // Your accept logic here, using the userId
        }



        public void AdditionalSettingsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("AdditionalSettings");
        }
        public void DeleteProjectConfirmClick(object sender, RoutedEventArgs e)
        {
            string error;
            var password = GetTemplateControl<PasswordBox>("DeleteProjectConfirmPasswordPasswordBox").Password;
            password = Security.HashText(password, Encoding.UTF8);
            if (password == UserManager.Instance.CurrentSessionUser.Password)
            {
                // Delete the project
                ProjectManager.Instance.DeleteProject(project.ProjectId);

                // Close any active ManageProjectWindow
                foreach (Window window in System.Windows.Application.Current.Windows)
                {
                    if (window is ManageProjectWindow)
                    {
                        window.Close();
                    }
                }

                // Check for active ProjectManagementWindow
                bool projectManagementWindowFound = false;
                foreach (Window window in System.Windows.Application.Current.Windows)
                {
                    if (window is ProjectManagementWindow)
                    {
                        projectManagementWindowFound = true;
                        window.Close();
                    }
                }

                // If no ProjectManagementWindow is found, display a new one
                if (!projectManagementWindowFound)
                {
                    ProjectManagementWindow newWindow = new ProjectManagementWindow();
                    newWindow.Show();
                }

                Close();
            }
            else
            {
                TextBlock Error = GetTemplateControl<TextBlock>("Error");
                Error.Text = "Password is incorrect";
            }
        }

        public void DeleteProjectClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("DeleteProject");

        }

        public void OverviewCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchPageTemplate("Overview");

        }

        public void OverviewConfirmClick(object sender, RoutedEventArgs e)
        {
            OverviewConfirm();
        }

        private void OverviewConfirm()
        {
            User data;
            var newProjName = GetTemplateControl<TextBox>("OverviewChangeProjectNameTextBox").Text;
            var password = GetTemplateControl<PasswordBox>("OverviewConfirmPassword").Password;
            password = Security.HashText(password, Encoding.UTF8);
            TextBlock overviewError = GetTemplateControl<TextBlock>("Error");
            data = new();
            data.Login = UserManager.Instance.CurrentSessionUser.Login;
            data.Password = password;




            bool success = UserManager.Instance.LoginUser(data);
            if (success == false)
                overviewError.Text = "Password is invalid";
            else
            {
                overviewError.Foreground = Brushes.White;
                overviewError.Text = "Project's name has been changed";
                Project updatedProject = project;
                updatedProject.Name = newProjName;
                ProjectManager.Instance.UpdateProject(updatedProject);
                var window = new AdminPanelWindow(updatedProject, backToProject);
                window.Activate();
                window.Visibility = Visibility.Visible;

                Close();
            }
        }

        public void UsersAddUserClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UsersAddUser");
        }
        public void UserDeleteConfirmCancelClick(object sender, RoutedEventArgs e)
        {
            TextBlock overviewError = GetTemplateControl<TextBlock>("Error");
            SwitchBackToPreviousTemplate();
        }


        public void DeleteUserFromProjectClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserDeleteConfirm");
        }

        public void UsersAddUserConfirmClick(object sender, RoutedEventArgs e)
        {
            var credential = GetTemplateControl<TextBox>("UsersAddUserSearchByUsernameTextBox").Text;
            User data = new();
            data.Login = credential;
            data.Email = credential;
            bool successUE = UserManager.Instance.UserExists(data, out User user);
            if (successUE)
            {
                //basically initializing AP with project edition
                ProjectManager.Instance.AddProjectMember(project.ProjectId, user.UserId);
            }
            //SwitchPageTemplate("UsersAddUserConfirm");

            SwitchBackToPreviousTemplate();
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            if (Error != null && successUE)
            {
                Error.Text = "User has been added successfully";
                Error.Foreground = Brushes.White;
            }
            else
            {
                Error.Text = "An error has occured";
            }




        }
        public void AppearanceClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Appearance");
        }

        public void UsersAddUserCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void UserPageAddNewTaskToUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserPageAddExistingTaskToUserClick(object sender, RoutedEventArgs e)
        {

        }


        public void EditUserPermsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserEditPermissions");
        }

        public void EditUserTasksClick(object sender, RoutedEventArgs e)
        {

        }


        public void UserPageCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void UserPageConfirmClick(object sender, RoutedEventArgs e)
        {

        }



        public void UserDeleteConfirmConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void UserNewTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAssignMoreUsersClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserNewTaskAddAnotherUser");
        }

        public void UserNewTaskAssignToExistingTaskListClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAssignToNewTaskListClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAddAnotherUserCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void UserNewTaskAddAnotherUserConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        /*public void UserNewTaskNewTaskListLeaderClick(object sender, RoutedEventArgs e)
        {

        }*/

        public void UserNewTaskExistingTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void UserNewTaskExistingTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserAssignExistingTaskCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void UserAssignExistingTaskConfirmClick(object sender, RoutedEventArgs e)
        {
            EFModeling.EntityProperties.DataAnnotations.Annotations.Task data = new EFModeling.EntityProperties.DataAnnotations.Annotations.Task();


        }

        public void UserEditPermissionsManagementLevelClick(object sender, RoutedEventArgs e)
        {
            //TU
            SwitchPageTemplate("UserEditPermissionsManagementLevel");
        }

        public void UserEditPermissionsEditManageRightsClick(object sender, RoutedEventArgs e)
        {
            //TU
            SwitchPageTemplate("UserEditPermissions");
        }

        public void UserEditPermissionsTransferProjectCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchPageTemplate("Overview");
        }

        public void UserEditPermissionsTransferProjectConfirmClick(object sender, RoutedEventArgs e)
        {
            var credential = GetTemplateControl<TextBox>("DeleteProjectConfirmPasswordText").Text;
            User current = UserManager.Instance.CurrentSessionUser;
            credential = Security.HashText(credential, Encoding.UTF8);
            TextBlock error = GetTemplateControl<TextBlock>("UserEditPermissionsTransferProject");
            if (credential == current.Password)
            {
                if (error == null)
                {
                    bool success = UserManager.Instance.GetUser(project.ManagerId, out User manager);
                    if (success)
                    {


                        if (UserManager.Instance.CurrentSessionUser.Admin != false)


                            error.Text = "Rights transferred";
                    }
                    else
                    {
                        error.Text = "An error has occured";
                    }
                }
            }
            else
            {
                error.Text = "Password is incorrect";
            }

            //SwitchPageTemplate("UserEditPermissionsTransferProject");

        }

        //THESE WILL BE USED LATER (after handing in desktop)
        public void UserEditPermissionsManagementLevelManagerClick(object sender, RoutedEventArgs e)
        {
            //MANAGER
            SwitchPageTemplate("UserEditPermissionsManagementLevelManager");
        }

        public void UserEditPermissionsManagementLevelTasklistLeaderClick(object sender, RoutedEventArgs e)
        {
            //TL LEADER
            SwitchPageTemplate("UserEditPermissionsManagementLevelTasklistLeader");
        }

        public void UserEditPermissionsManagementLevelTasklLeaderClick(object sender, RoutedEventArgs e)
        {
            //TASK LEADER
            SwitchPageTemplate("UserEditPermissionsManagementLevelTaskLeader");
        }

        public void UserEditPermissionsManagementLevelTransferProjectRightsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserEditPermissionsTransferProject");
        }

        public void UserEditPermissionsManagementLevelCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void UserEditPermissionsManagementLevelConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTasksConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTasksCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void UserTasksRemoveTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserTasksRemoveTasks");
        }

        public void UserTasksNewTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserNewTask");
        }

        public void UserTasksRemoveTasksConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserTasksRemoveTasksCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();


        }

        public void TasksNewTaskListClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskList");
        }

        public void TasksNewTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTask");
        }

        public void TasksAddTaskListConfirmClick(object sender, RoutedEventArgs e)
        {
            TaskList data = new TaskList();
            data.ProjectId = project.ProjectId;

            var taskListName = GetTemplateControl<TextBox>("TasksAddTaskListNameTextBox").Text;
            var taskListDesc = GetTemplateControl<TextBox>("TasksAddTaskListDescriptionTextBox").Text;
            TextBlock taskListErr = GetTemplateControl<TextBlock>("Error");
            if (taskListName != String.Empty)
            {
                data.Name = taskListName;
                data.Description = taskListDesc;
                TaskListManager.Instance.CreateTaskList(data);
                SwitchBackToPreviousTemplate();

                taskListErr.Foreground = Brushes.White;
                taskListErr.Text = "Task list has been created";
            }
            else if (taskListName == String.Empty)
            {
                taskListErr.Foreground = Brushes.Red;
                taskListErr.Text = "Insert task list's name";
            }
            else
            {
                taskListErr.Foreground = Brushes.Red;
                taskListErr.Text = "An error has occured";
            }
        }

        public void TasksAddTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }
        public void MeetingsAddMeetingCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }
        public void MeetingsAddMeetingConfirmClick(object sender, RoutedEventArgs e)
        {
            Meeting data = new Meeting();
            data.ProjectId = project.ProjectId;
            TextBlock Err = GetTemplateControl<TextBlock>("Error");
            var meetingTitle = GetTemplateControl<TextBox>("TasksAddTaskListNameTextBox").Text;
            var meetingDesc = GetTemplateControl<TextBox>("TasksAddTaskListDescriptionTextBox").Text;
            var meetingDate = GetTemplateControl<TextBox>("TasksAddTaskDeadlineTextBox").Text;
            bool isDateValid = DateTime.TryParse(meetingDate, out DateTime deadline);
            if (meetingDate == String.Empty)
            {
                Err.Foreground = Brushes.Red;
                Err.Text = "Insert meeting's date";
                return;
            }
            else if (meetingTitle == String.Empty)
            {
                Err.Foreground = Brushes.Red;
                Err.Text = "Insert meeting's name";
                return;
            }
            else if (meetingTitle != String.Empty && isDateValid)
            {
                data.Title = meetingTitle;
                data.Description = meetingDesc;
                
                MeetingManager.Instance.CreateMeeting(data);
                SwitchBackToPreviousTemplate();

                Err.Foreground = Brushes.White;
                Err.Text = "Task list has been created";
            }
            
            else
            {
                Err.Foreground = Brushes.Red;
                Err.Text = "An error has occured";
                return;
            }
        }

        //left it uncommented since it's just switching ContentTemplates
        public void TasksAddTaskListLeaderClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskListAssignLeader");
        }


        /*public void TasksAddTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
        }*/

        /*public void TasksAddTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }*/
        public void TasksEditTaskClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (long.TryParse(button.Tag.ToString(), out long taskId))
                {
                    currentlyEditedTask = TaskManager.Instance.GetTask(taskId);
                    SwitchPageTemplate("TasksEditTask");
                }
                else
                {
                    // Handle the case where the tag could not be parsed to a long
                    // e.g., display an error message or log the issue
                }
            }
        }

        public void PutProjectNameInText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Assuming you have a project object with a Name property
                string projectName = project.Name; // Replace this with your actual project object and property
                textBox.Text = projectName;
            }

        }


        public void PutAssignedUsersCount(object sender, RoutedEventArgs e)
        {
            List<User> members = TaskManager.Instance.GetMembers(currentlyEditedTask.TaskId);
            if (sender is TextBlock textBlock)
            {
                textBlock.Text = $"Users assigned:{members.Count}";
            }
        }
        public void TasksEditTaskListConfirmClick(object sender, RoutedEventArgs e)
        {
            TaskList tasklist = new TaskList();
            var taskListName = GetTemplateControl<TextBox>("TasksEditTaskListNameTextBox").Text;
            var taskListDesc = GetTemplateControl<TextBox>("TasksEditTaskListDescriptionTextBox").Text;
            tasklist.Name = taskListName;
            tasklist.Description = taskListDesc;
            tasklist.TaskListId = currentlyEditedTaskList.TaskListId;
            var err = GetTemplateControl<TextBlock>("Error");
            if (taskListName == String.Empty)
            {
                err.Foreground = Brushes.Red;
                err.Text = "Task list must have a name";
                return;
            }
            
            bool success = TaskListManager.Instance.UpdateTaskList(tasklist);
            if (success)
            {
                err.Foreground = Brushes.White;
                err.Text = "Task list updated successfully";
            }
            else
            {
                err.Foreground = Brushes.Red;
                err.Text = "An error has occured";
            }

        }

        public void TasksEditTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void TasksEditTaskAssignUsersToTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskViewUsers");
        }

        public void TasksEditTaskListUserListClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskViewUsers");
        }

        public void TasksEditTaskListClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (long.TryParse(button.Tag.ToString(), out long tasklistId))
                {
                    currentlyEditedTaskList = TaskListManager.Instance.GetTaskList(tasklistId);
                    SwitchPageTemplate("TasksEditTaskList");
                }
                else
                {
                    // Handle the case where the tag could not be parsed to a long
                    // e.g., display an error message or log the issue
                }
            }
        }
        public void MeetingsEditMeetingClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (long.TryParse(button.Tag.ToString(), out long meetingId))
                {
                    bool success = MeetingManager.Instance.GetMeeting(meetingId, out Meeting meeting);
                    currentlyEditedMeeting = meeting;
                    SwitchPageTemplate("MeetingsEditMeeting");
                }
                else
                {
                    // Handle the case where the tag could not be parsed to a long
                    // e.g., display an error message or log the issue
                }
            }
        }

        /*public void TasksEditTaskListChangeLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskListChangeLeader");
        }

        public void TasksEditTaskListChangeLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
        }*/
        public void PutTaskPropertyInTextBox(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                switch (textBox.Tag)
                {
                    case "Description":
                        textBox.Text = currentlyEditedTask.Description;
                        break;

                    case "Name":
                        textBox.Text = currentlyEditedTask.Name;
                        break;

                    default:
                        // Handle any other cases, if needed
                        break;
                }
            }
        }
        public void PutTaskListPropertyInTextBox(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                switch (textBox.Tag)
                {
                    case "Description":
                        textBox.Text = currentlyEditedTaskList.Description;
                        break;

                    case "Name":
                        textBox.Text = currentlyEditedTaskList.Name;
                        break;

                    default:
                        // Handle any other cases, if needed
                        break;
                }
            }
        }

        public void PutMeetingPropertyInControl(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox || sender is DateTimePicker dateTimePicker)
            {
                var control = sender as FrameworkElement;

                switch (control.Tag)
                {
                    case "Description":
                        if (control is TextBox descriptionTextBox)
                            descriptionTextBox.Text = currentlyEditedMeeting.Description;
                        break;

                    case "Title":
                        if (control is TextBox titleTextBox)
                            titleTextBox.Text = currentlyEditedMeeting.Title;
                        break;

                    case "Date":
                        if (control is TextBox dateTextBox)
                            dateTextBox.Text = currentlyEditedMeeting.Date.ToString("yyyy-MM-dd HH:mm");
                        else if (control is DateTimePicker dateTimePickerControl)
                            dateTimePickerControl.Value = currentlyEditedMeeting.Date;
                        break;

                    default:
                        // Handle any other cases, if needed
                        break;
                }
            
            }
        }

        public void SetNewTaskButtonVisibility(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                List<TaskList> taskLists;
                string error = String.Empty;
                bool success = TaskListManager.Instance.GetAllTaskLists(project.ProjectId, out taskLists);

                if (success)
                {
                    if (taskLists.Count > 0)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        public void MeetingsNewMeetingClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("MeetingsNewMeeting");
        }

            public void TasksViewTaskListUsersAddUserClick(object sender, RoutedEventArgs e)
        {

        }

        private void PutTaskListName(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                // Assuming you have a way to get the taskList name
                if (currentlyEditedTask != null) {
                    TaskList taskList = TaskListManager.Instance.GetTaskList(currentlyEditedTask.TaskListId); // Replace this with your actual method to get the task list name
                    textBlock.Text = $"Task list: {taskList.Name}";
                }
                if (currentlyEditedTaskList != null)
                {
                    TaskList taskList = currentlyEditedTaskList; // Replace this with your actual method to get the task list name
                    textBlock.Text = $"Task list: {taskList.Name}";
                }


            }
        }
        private void PutTaskListID(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                // Assuming you have a way to get the taskList name
                if (currentlyEditedTask != null)
                {
                    TaskList taskList = TaskListManager.Instance.GetTaskList(currentlyEditedTask.TaskListId); // Replace this with your actual method to get the task list name
                    textBlock.Text = $"ID: {taskList.TaskListId}";
                }
                if (currentlyEditedTaskList != null)
                {
                    TaskList taskList = currentlyEditedTaskList; // Replace this with your actual method to get the task list name
                    textBlock.Text = $"ID: {taskList.TaskListId}";
                }
            }
        }


        public void TasksAddTaskConfirmClick(object sender, RoutedEventArgs e)
        {
            EFModeling.EntityProperties.DataAnnotations.Annotations.Task data = new EFModeling.EntityProperties.DataAnnotations.Annotations.Task();
            var taskName = GetTemplateControl<TextBox>("TasksAddTaskNameTextBox").Text;
            var taskDesc = GetTemplateControl<TextBox>("TasksAddTaskDescriptionTextBox").Text;
            var taskDeadline = GetTemplateControl<TextBox>("TasksAddTaskDeadlineTextBox").Text;
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            bool isDeadlineValid = DateTime.TryParse(taskDeadline, out DateTime deadline);
            if (taskDeadline == String.Empty)
            {
                Error.Text = "Insert task's deadline";
                return;
            }
            else if (taskName == String.Empty)
            {
                Error.Text = "Insert task's name";
            }
            if (isDeadlineValid && taskName != "")
            {
                data = new();
                data.Name = taskName;
                data.Description = taskDesc;
                data.Deadline = deadline;
                TaskManager.Instance.CreateTask(data);

                //SwitchPageTemplate("UsersAddUserConfirm");

                SwitchBackToPreviousTemplate();

                Error.Text = "Task has been created successfully";
                Error.Foreground = Brushes.White;
            }
            
            else
            {
                Error.Text = "An error has occured";
            }


        }

        public void TasksAddTaskCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void TasksAddTaskAssignTaskListNewClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskList");
        }

        public void TasksAddTaskAssignTaskListExistingClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskAssignTaskListExisting");
        }


        public void TasksAddTaskNewTaskListConfirmClick(object sender, RoutedEventArgs e)
        {
            TaskList data = new TaskList();
            var taskListName = GetTemplateControl<TextBox>("TasksAddTaskListNameTextBox").Text;
            var taskListDesc = GetTemplateControl<TextBox>("TasksAddTaskListDescriptionTextBox").Text;
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            if (taskListName != String.Empty)
            {
                data = new();
                data.Name = taskListName;
                data.Description = taskListDesc;
                TaskListManager.Instance.CreateTaskList(data);

                //SwitchPageTemplate("UsersAddUserConfirm");

                SwitchBackToPreviousTemplate();

                Error.Text = "Task has been created successfully";
                Error.Foreground = Brushes.White;
            }
            else
            {
                Error.Text = "Insert task list's name";
            }
        }

        /*
        public void TasksAddTaskNewTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskNewTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();

        }*/

        public void TasksAddTaskAddUsersConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskAddUsers");
        }

        public void TasksAddTaskAddUsersCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        /*public void TasksAddTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskChangeLeader");
        }

        public void TasksAddTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }*/

        public void TasksEditTaskConfirmClick(object sender, RoutedEventArgs e)
        {
            // Assuming the UI has text boxes or controls to get updated task details.
            var updatedTaskName = GetTemplateControl<TextBox>("TasksEditTaskNameTextBox");
            var updatedTaskDesc = GetTemplateControl<TextBox>("TasksEditTaskDescriptionTextBox");
            var updatedTaskDeadline = GetTemplateControl<TextBox>("TasksEditDeadlineTextBox");
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            bool isDeadlineValid = DateTime.TryParse(updatedTaskDeadline.Text, out DateTime deadline);

            if (!isDeadlineValid)
            {
                System.Windows.MessageBox.Show("Invalid deadline date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get the task ID from the selected item (assuming it's stored in a hidden field or data context).
            long taskId = 0; // Assuming this is where the task ID should be retrieved
                             // long taskId = (long)hiddenTaskId.Value; // Uncomment this line if you have hiddenTaskId available

            // Instantiate the TaskManager if it hasn't been done already.
            if (TaskManager.Instance == null)
            {
                TaskManager.Instantiate();
            }

            bool taskExists = TaskManager.Instance.GetTaskId(taskListId: 1, name: updatedTaskName.Text, out taskId);

            if (taskExists)
            {
                // Create a new Task object with updated details
                EFModeling.EntityProperties.DataAnnotations.Annotations.Task updatedTask = new EFModeling.EntityProperties.DataAnnotations.Annotations.Task
                {
                    TaskId = taskId,
                    Name = updatedTaskName.Text,
                    Description = updatedTaskDesc.Text,
                    Deadline = deadline
                };

                // Update the task
                bool success = TaskManager.Instance.UpdateTask(updatedTask);
                if (success)
                {
                    //we can also move to previous template and forward it to Error TextBlock

                    Error.Foreground = Brushes.White;
                    Error.Text = "Task updated successfully";
                }
                else
                {
                    Error.Foreground = Brushes.Red;
                    Error.Text = "Failed to update task";
                }
            }
            else
            {
                SwitchBackToPreviousTemplate();
                System.Windows.MessageBox.Show("Task not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void MeetingsEditMeetingConfirmClick(object sender, RoutedEventArgs e)
        {
            // Assuming the UI has text boxes or controls to get updated task details.
            var updatedTaskName = GetTemplateControl<TextBox>("TasksEditTaskNameTextBox");
            var updatedTaskDesc = GetTemplateControl<TextBox>("TasksEditTaskDescriptionTextBox");
            var updatedTaskDeadline = GetTemplateControl<TextBox>("TasksEditDeadlineTextBox");
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            bool isDeadlineValid = DateTime.TryParse(updatedTaskDeadline.Text, out DateTime deadline);

            if (!isDeadlineValid)
            {
                System.Windows.MessageBox.Show("Invalid deadline date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get the task ID from the selected item (assuming it's stored in a hidden field or data context).
            long taskId = 0; // Assuming this is where the task ID should be retrieved
                             // long taskId = (long)hiddenTaskId.Value; // Uncomment this line if you have hiddenTaskId available

            // Instantiate the TaskManager if it hasn't been done already.
            if (TaskManager.Instance == null)
            {
                TaskManager.Instantiate();
            }

            bool taskExists = TaskManager.Instance.GetTaskId(taskListId: 1, name: updatedTaskName.Text, out taskId);

            if (taskExists)
            {
                // Create a new Task object with updated details
                EFModeling.EntityProperties.DataAnnotations.Annotations.Task updatedTask = new EFModeling.EntityProperties.DataAnnotations.Annotations.Task
                {
                    TaskId = taskId,
                    Name = updatedTaskName.Text,
                    Description = updatedTaskDesc.Text,
                    Deadline = deadline
                };

                // Update the task
                bool success = TaskManager.Instance.UpdateTask(updatedTask);
                if (success)
                {
                    //we can also move to previous template and forward it to Error TextBlock

                    Error.Foreground = Brushes.White;
                    Error.Text = "Task updated successfully";
                }
                else
                {
                    Error.Foreground = Brushes.Red;
                    Error.Text = "Failed to update task";
                }
            }
            else
            {
                SwitchBackToPreviousTemplate();
                System.Windows.MessageBox.Show("Task not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public void CancelTaskRemoveClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }
        public void ConfirmTaskRemoveClick(object sender, RoutedEventArgs e)
        {
            TaskManager.Instance.DeleteTask(currentlyEditedTask.TaskId);
            currentlyEditedTask = null;
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Tasks");
        }
        public void CancelTaskListRemoveClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }
        public void ConfirmTaskListRemoveClick(object sender, RoutedEventArgs e)
        {
            TaskListManager.Instance.DeleteTaskList(currentlyEditedTaskList.TaskListId);
            currentlyEditedTask = null;
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Tasks");
        }
        public void CancelMeetingRemoveClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }
        public void ConfirmMeetingRemoveClick(object sender, RoutedEventArgs e)
        {
            MeetingManager.Instance.DeleteMeeting(currentlyEditedMeeting.MeetingId);
            currentlyEditedTask = null;
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Meeting");
        }

        public void TasksEditTaskCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }
        public void TasksEditTaskListDeleteClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskListDelete");
        }

        public void TasksEditTaskDeleteTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTasksDelete");
        }
        public void MeetingsEditMeetingsDeleteClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("MeetingsEditMeetingDelete");
        }

        public void TasksEditTaskViewUsersAddClick(object sender, RoutedEventArgs e)
        {
            // Assume the UI has controls to get UserId and TaskId
            int userId = int.Parse(GetTemplateControl<TextBox>("UserTextBox").Text);
            int taskId = int.Parse(GetTemplateControl<TextBox>("TaskTextBox").Text);

            // Instantiate the TaskManager if it hasn't been done already.
            if (TaskManager.Instance == null)
            {
                TaskManager.Instantiate();
            }

            // Create a new TaskDetails object
            TaskDetails taskDetails = new TaskDetails
            {
                UserId = userId,
                TaskId = taskId
            };

            bool success = true;
            if (success)
            {
                System.Windows.MessageBox.Show("User added to task successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                TaskManager.Instance.CreateTaskDetails(taskDetails);
            }
            else
            {
                System.Windows.MessageBox.Show("Failed to add user to task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void TasksEditTaskViewUsersRemoveClick(object sender, RoutedEventArgs e)
        {
            // Assume the UI has controls to get UserId and TaskId
            int userId = int.Parse(GetTemplateControl<TextBox>("UserIdTextBox").Text);
            int taskId = int.Parse(GetTemplateControl<TextBox>("TaskIdTextBox").Text);

            // Instantiate the TaskManager if it hasn't been done already.
            if (TaskManager.Instance == null)
            {
                TaskManager.Instantiate();
            }

            // Create a new TaskDetails object
            TaskDetails taskDetails = new TaskDetails
            {
                UserId = userId,
                TaskId = taskId
            };
            bool success = true;
            if (success)
            {
                TaskManager.Instance.ClearTaskDetails(taskDetails);
                System.Windows.MessageBox.Show("User removed from task successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                

            }
            else
            {
                System.Windows.MessageBox.Show("Failed to remove user from task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void TasksEditTaskAddUsersConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskAddUsersConfirm");
        }

        public void TasksEditTaskAddUsersCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void TasksEditTaskDeleteUsersAddClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskDeleteUsersRemoveClick(object sender, RoutedEventArgs e)
        {

        }

        /*public void TasksEditTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
        }

        public void TasksEditTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
        //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
            SwitchBackToPreviousTemplate();
        }*/

        public void SettingsWindowReset(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Overview");
            //ResetTextInputs(); imo there should be logic to remember basic settings but I guess we will just port it with each template load
        }

        public void SettingsWindowConfirm(object sender, RoutedEventArgs e)
        {
            //imo there should be logic that saves the settings into settings.json
        }
        /*public void TaskAddTaskListAssignLeaderConfirm(object sender, RoutedEventArgs e)
        {
            //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
        }
        public void TaskAddTaskListAssignLeaderCancel(object sender, RoutedEventArgs e)
        {
            //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
            SwitchBackToPreviousTemplate();
        }*/
        public void AccountClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Account");
        }
        public void ProjectsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Projects");
        }
        public void PrivacyClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Privacy");
        }
        public void MeetingsClick(object sender, RoutedEventArgs e)
        {
            if (templateHistory.Count > 0)
            {
                templateHistory.Clear();
            }
            SwitchPageTemplate("Meetings");
        }
        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("LogOut");
        }
    }
}
=======
﻿using EFModeling.EntityProperties.DataAnnotations.Annotations;
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



namespace Desktop
{
    public class AdminPanelViewModel
    {
        //EXAMPLE DATA
        public ObservableCollection<ProjectMembers> PendingInvites { get; set; } = new ObservableCollection<ProjectMembers> {
            new ProjectMembers { UserId = 1},
            new ProjectMembers { UserId = 2}
            //new ProjectMembers { UserId = 3}
        };
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /// <summary>
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {

        private Stack<ControlTemplate> templateHistory;
        Project project = new Project { ProjectId = 1, ManagerId = 1, Name = "Testing" };
        User pickedUser; //we will use it for editing specific user or tranferring rights to it.

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
        private void ProjectNameHeaderDispName(object sender, RoutedEventArgs e)
        {
            TextBlock header = GetTemplateControl<TextBlock>("ProjectNameHeader");
            //header.Text = project.Name;
            header.Text = project.Name;
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
                var window = new ManageProjectWindow(project);
                window.Activate();
                window.Visibility = Visibility.Visible;

                Close();
            }

        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void SendInvitationClick(object sender, RoutedEventArgs e)
        {
            TextBlock Error = GetTemplateControl<TextBlock>("Error");

            var credential = GetTemplateControl<TextBox>("UserToInvite").Text;
            User data = new();
            data.Login = credential;
            data.Email = credential;
            bool successUE = UserManager.Instance.UserExists(data, out User user);
            Error.Text = credential;
            if (successUE)
            {
                UserManager.Instance.SendProjectInvite(user, project);
                Error.Foreground = Brushes.White;
                Error.Text = "User has been invited";
            }
            else
            {
                Error.Foreground = Brushes.Red;
                Error.Text = "User hasn't been found";
            }

        }

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
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject { if (depObj != null) { for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) { DependencyObject child = VisualTreeHelper.GetChild(depObj, i); if (child != null && child is T) { yield return (T)child; } foreach (T childOfChild in FindVisualChildren<T>(child)) { yield return childOfChild; } } } }


        public AdminPanelWindow(Project project)
        {
            InitializeComponent();
            //add logic to lead picked project name

            templateHistory = new Stack<ControlTemplate>();
            DataContext = new AdminPanelViewModel();
            this.project = project;
        }


        public void BackClick(object sender, RoutedEventArgs e)
        {
            //prolly if in main adm panel window then do this.close()
            SwitchBackToPreviousTemplate();
        }

        public void OverviewClick(object sender, RoutedEventArgs e)
        {
            templateHistory.Clear();
            SwitchPageTemplate("Overview");
        }

        public void UsersClick(object sender, RoutedEventArgs e)
        {
            templateHistory.Clear();
            SwitchPageTemplate("Users");
        }

        public void SupportClick(object sender, RoutedEventArgs e)
        {
            templateHistory.Clear();
            SwitchPageTemplate("Support");
        }
        public void TasksClick(object sender, RoutedEventArgs e)
        {
            templateHistory.Clear();
            SwitchPageTemplate("Tasks");

        }



        public void SecurityClick(object sender, RoutedEventArgs e)
        {
            templateHistory.Clear();
            SwitchPageTemplate("Security");
        }

        public void AuditLogClick(object sender, RoutedEventArgs e)
        {
            templateHistory.Clear();
            SwitchPageTemplate("AuditLog");
        }

        public void InvitesClick(object sender, RoutedEventArgs e)
        {
            templateHistory.Clear();
            SwitchPageTemplate("Invites");

            // Assuming the DataContext is already set to an instance of AdminPanelViewModel
            if (DataContext is AdminPanelViewModel viewModel)
            {
                // Retrieve unaccepted invites (this example does not use them yet)
                List<ProjectMembers> unacceptedInvites = ProjectManager.Instance.GetUnacceptedInvites(project);

                // Optionally clear existing invites
                viewModel.PendingInvites.Clear();

                // Add pending
                // Add unaccepted invites to PendingInvites
                foreach (var invite in unacceptedInvites)
                {
                    viewModel.PendingInvites.Add(invite);
                }




                // Update the template or UI as needed
                SwitchPageTemplate("Invites");
            }
        }


        // Example methods for click events
        private void DeclineInviteButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int userId = (int)button.Tag;
            User user;
            UserManager.Instance.GetUser(userId, out user);
            ProjectManager.Instance.RemoveProjectMember(project, user);


            SwitchPageTemplate("Invites");
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            Error.Foreground = Brushes.Red;
            Error.Text = "Invite has been declined";

            // Your decline logic here, using the userId
        }

        private void AcceptInviteButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int userId = (int)button.Tag;
            User user;
            UserManager.Instance.GetUser(userId, out user);
            bool success = ProjectManager.Instance.AcceptInvite(project, user);
            if (success)
            {
                SwitchPageTemplate("Invites");
                TextBlock Error = GetTemplateControl<TextBlock>("Error");
                Error.Foreground = Brushes.White;
                Error.Text = "Invite has been accepted";
            }
            else
            {
                TextBlock Error = GetTemplateControl<TextBlock>("Error");
                Error.Foreground = Brushes.Red;
                Error.Text = "An error has occured";
            }
            // Your accept logic here, using the userId
        }



        public void AdditionalSettingsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("AdditionalSettings");
        }
        public void DeleteProjectConfirmClick(object sender, RoutedEventArgs e)
        {
            string error;
            var password = GetTemplateControl<PasswordBox>("DeleteProjectConfirmPasswordPasswordBox").Password;
            password = Security.HashText(password, Encoding.UTF8);
            if (password == UserManager.Instance.CurrentSessionUser.Password)
            { //delete project and close the window since I hv panelwindow with set project edit
                ProjectManager.Instance.DeleteProject(project.ProjectId);
                this.Close();
            }
            else
            {
                TextBlock Error = GetTemplateControl<TextBlock>("Error");
                Error.Text = "Password is incorrect";
            }
        }
        public void DeleteProjectClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("DeleteProject");

        }

        public void OverviewCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchPageTemplate("Overview");

        }

        public void OverviewConfirmClick(object sender, RoutedEventArgs e)
        {
            OverviewConfirm();
        }

        private void OverviewConfirm()
        {
            User data;
            var newProjName = GetTemplateControl<TextBox>("OverviewChangeProjectNameTextBox").Text;
            var password = GetTemplateControl<PasswordBox>("OverviewConfirmPassword").Password;
            password = Security.HashText(password, Encoding.UTF8);
            TextBlock overviewError = GetTemplateControl<TextBlock>("Error");
            data = new();
            data.Login = UserManager.Instance.CurrentSessionUser.Login;
            data.Password = password;




            bool success = UserManager.Instance.LoginUser(data);
            if (success == false)
                overviewError.Text = "Password is invalid";
            else
            {
                overviewError.Foreground = Brushes.White;
                overviewError.Text = "Project's name has been changed";
            }
        }

        public void UsersAddUserClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UsersAddUser");
        }
        public void UserDeleteConfirmCancelClick(object sender, RoutedEventArgs e)
        {
            TextBlock overviewError = GetTemplateControl<TextBlock>("Error");
            SwitchBackToPreviousTemplate();
        }


        public void DeleteUserFromProjectClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserDeleteConfirm");
        }

        public void UsersAddUserConfirmClick(object sender, RoutedEventArgs e)
        {
            var credential = GetTemplateControl<TextBox>("UsersAddUserSearchByUsernameTextBox").Text;
            User data = new();
            data.Login = credential;
            data.Email = credential;
            bool successUE = UserManager.Instance.UserExists(data, out User user);
            if (successUE)
            {
                //basically initializing AP with project edition
                ProjectManager.Instance.AddProjectMember(project.ProjectId, user.UserId);
            }
            //SwitchPageTemplate("UsersAddUserConfirm");

            SwitchBackToPreviousTemplate();
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            if (Error != null && successUE)
            {
                Error.Text = "User has been added successfully";
                Error.Foreground = Brushes.White;
            }
            else
            {
                Error.Text = "An error has occured";
            }




        }
        public void AppearanceClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Appearance");
        }

        public void UsersAddUserCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void UserPageAddNewTaskToUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserPageAddExistingTaskToUserClick(object sender, RoutedEventArgs e)
        {

        }


        public void EditUserPermsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserEditPermissions");
        }

        public void EditUserTasksClick(object sender, RoutedEventArgs e)
        {

        }


        public void UserPageCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void UserPageConfirmClick(object sender, RoutedEventArgs e)
        {

        }



        public void UserDeleteConfirmConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void UserNewTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAssignMoreUsersClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserNewTaskAddAnotherUser");
        }

        public void UserNewTaskAssignToExistingTaskListClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAssignToNewTaskListClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAddAnotherUserCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void UserNewTaskAddAnotherUserConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        /*public void UserNewTaskNewTaskListLeaderClick(object sender, RoutedEventArgs e)
        {

        }*/

        public void UserNewTaskExistingTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void UserNewTaskExistingTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserAssignExistingTaskCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void UserAssignExistingTaskConfirmClick(object sender, RoutedEventArgs e)
        {
            EFModeling.EntityProperties.DataAnnotations.Annotations.Task data = new EFModeling.EntityProperties.DataAnnotations.Annotations.Task();


        }

        public void UserEditPermissionsManagementLevelClick(object sender, RoutedEventArgs e)
        {
            //TU
            SwitchPageTemplate("UserEditPermissionsManagementLevel");
        }

        public void UserEditPermissionsEditManageRightsClick(object sender, RoutedEventArgs e)
        {
            //TU
            SwitchPageTemplate("UserEditPermissions");
        }

        public void UserEditPermissionsTransferProjectCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchPageTemplate("Overview");
        }

        public void UserEditPermissionsTransferProjectConfirmClick(object sender, RoutedEventArgs e)
        {
            var credential = GetTemplateControl<TextBox>("DeleteProjectConfirmPasswordText").Text;
            User current = UserManager.Instance.CurrentSessionUser;
            credential = Security.HashText(credential, Encoding.UTF8);
            TextBlock error = GetTemplateControl<TextBlock>("UserEditPermissionsTransferProject");
            if (credential == current.Password)
            {
                if (error == null)
                {
                    bool success = UserManager.Instance.GetUser(project.ManagerId, out User manager);
                    if (success)
                    {


                        if (UserManager.Instance.CurrentSessionUser.Admin != false)


                            error.Text = "Rights transferred";
                    }
                    else
                    {
                        error.Text = "An error has occured";
                    }
                }
            }
            else
            {
                error.Text = "Password is incorrect";
            }

            //SwitchPageTemplate("UserEditPermissionsTransferProject");

        }

        //THESE WILL BE USED LATER (after handing in desktop)
        public void UserEditPermissionsManagementLevelManagerClick(object sender, RoutedEventArgs e)
        {
            //MANAGER
            SwitchPageTemplate("UserEditPermissionsManagementLevelManager");
        }

        public void UserEditPermissionsManagementLevelTasklistLeaderClick(object sender, RoutedEventArgs e)
        {
            //TL LEADER
            SwitchPageTemplate("UserEditPermissionsManagementLevelTasklistLeader");
        }

        public void UserEditPermissionsManagementLevelTasklLeaderClick(object sender, RoutedEventArgs e)
        {
            //TASK LEADER
            SwitchPageTemplate("UserEditPermissionsManagementLevelTaskLeader");
        }

        public void UserEditPermissionsManagementLevelTransferProjectRightsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserEditPermissionsTransferProject");
        }

        public void UserEditPermissionsManagementLevelCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void UserEditPermissionsManagementLevelConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTasksConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTasksCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void UserTasksRemoveTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserTasksRemoveTasks");
        }

        public void UserTasksNewTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UserNewTask");
        }

        public void UserTasksRemoveTasksConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserTasksRemoveTasksCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();


        }

        public void TasksNewTaskListClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskList");
        }

        public void TasksNewTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTask");
        }

        public void TasksAddTaskListConfirmClick(object sender, RoutedEventArgs e)
        {
            TaskList data = new TaskList();
            var taskListName = GetTemplateControl<TextBox>("TasksAddTaskListNameTextBox").Text;
            var taskListDesc = GetTemplateControl<TextBox>("TasksAddTaskListDescriptionTextBox").Text;
            TextBox taskListErr = GetTemplateControl<TextBox>("Error");
            if (taskListName != String.Empty)
            {

                TaskListManager.Instance.CreateTaskList(data);
                SwitchBackToPreviousTemplate();

                taskListErr.Foreground = Brushes.White;
                taskListErr.Text = "Task list has been created";
            }
            else if (taskListName == String.Empty)
            {
                taskListErr.Foreground = Brushes.Red;
                taskListErr.Text = "Insert project's name";
            }
            else
            {
                taskListErr.Foreground = Brushes.Red;
                taskListErr.Text = "An error has occured";
            }
        }

        public void TasksAddTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        //left it uncommented since it's just switching ContentTemplates
        public void TasksAddTaskListLeaderClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskListAssignLeader");
        }


        /*public void TasksAddTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
        }*/

        /*public void TasksAddTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }*/

        public void TasksEditTaskListConfirmClick(object sender, RoutedEventArgs e)
        {
            TaskList tasklist = new TaskList();
            var taskListName = GetTemplateControl<TextBox>("TasksAddTaskListNameTextBox").Text;
            var taskListDesc = GetTemplateControl<TextBox>("TasksAddTaskListDescriptionTextBox").Text;
            TaskListManager.Instance.UpdateTaskList(tasklist);

        }

        public void TasksEditTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
        }

        public void TasksEditTaskAssignUsersToTaskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskViewUsers");
        }

        public void TasksEditTaskListUserListClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskViewUsers");
        }

        public void TasksEditTaskListDeleteClick(object sender, RoutedEventArgs e)
        {

        }

        /*public void TasksEditTaskListChangeLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskListChangeLeader");
        }

        public void TasksEditTaskListChangeLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
        }*/

        public void TasksViewTaskListUsersAddUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskConfirmClick(object sender, RoutedEventArgs e)
        {
            EFModeling.EntityProperties.DataAnnotations.Annotations.Task data = new EFModeling.EntityProperties.DataAnnotations.Annotations.Task();
            var taskName = GetTemplateControl<TextBox>("TasksAddTaskNameTextBox").Text;
            var taskDesc = GetTemplateControl<TextBox>("TasksAddTaskDescriptionTextBox").Text;
            var taskDeadline = GetTemplateControl<TextBox>("TasksAddTaskDeadlineTextBox").Text;
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            bool isDeadlineValid = DateTime.TryParse(taskDeadline, out DateTime deadline);
            if (isDeadlineValid && taskName != "")
            {
                data = new();
                data.Name = taskName;
                data.Description = taskDesc;
                data.Deadline = deadline;
                TaskManager.Instance.CreateTask(data);

                //SwitchPageTemplate("UsersAddUserConfirm");

                SwitchBackToPreviousTemplate();

                Error.Text = "Task has been created successfully";
                Error.Foreground = Brushes.White;
            }
            else if (taskName == String.Empty)
            {
                Error.Text = "Insert task's name";
            }
            else
            {
                Error.Text = "An error has occured";
            }


        }

        public void TasksAddTaskCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void TasksAddTaskAssignTaskListNewClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskList");
        }

        public void TasksAddTaskAssignTaskListExistingClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskAssignTaskListExisting");
        }


        public void TasksAddTaskNewTaskListConfirmClick(object sender, RoutedEventArgs e)
        {
            TaskList data = new TaskList();
            var taskListName = GetTemplateControl<TextBox>("TasksAddTaskListNameTextBox").Text;
            var taskListDesc = GetTemplateControl<TextBox>("TasksAddTaskListDescriptionTextBox").Text;
            TextBlock Error = GetTemplateControl<TextBlock>("Error");
            if (taskListName != String.Empty)
            {
                data = new();
                data.Name = taskListName;
                data.Description = taskListDesc;
                TaskListManager.Instance.CreateTaskList(data);

                //SwitchPageTemplate("UsersAddUserConfirm");

                SwitchBackToPreviousTemplate();

                Error.Text = "Task has been created successfully";
                Error.Foreground = Brushes.White;
            }
            else
            {
                Error.Text = "Insert task list's name";
            }
        }

        /*
        public void TasksAddTaskNewTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskNewTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();

        }*/

        public void TasksAddTaskAddUsersConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskAddUsers");
        }

        public void TasksAddTaskAddUsersCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        /*public void TasksAddTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskChangeLeader");
        }

        public void TasksAddTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }*/

        public void TasksEditTaskConfirmClick(object sender, RoutedEventArgs e)
        {
            // Assuming the UI has text boxes or controls to get updated task details.
            var updatedTaskName = GetTemplateControl<TextBox>("TasksEditNameTextBox").Text;
            var updatedTaskDesc = GetTemplateControl<TextBox>("TasksEditDescriptionTextBox").Text;
            var updatedTaskDeadline = GetTemplateControl<TextBox>("TasksEditDeadlineTextBox").Text;
            bool isDeadlineValid = DateTime.TryParse(updatedTaskDeadline, out DateTime deadline);

            if (!isDeadlineValid)
            {
                MessageBox.Show("Invalid deadline date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get the task ID from the selected item (assuming it's stored in a hidden field or data context).
            long taskId = 0; // Assuming this is where the task ID should be retrieved
                             // long taskId = (long)hiddenTaskId.Value; // Uncomment this line if you have hiddenTaskId available

            // Instantiate the TaskManager if it hasn't been done already.
            if (TaskManager.Instance == null)
            {
                TaskManager.Instantiate();
            }

            bool taskExists = TaskManager.Instance.GetTaskId(taskListId: 1, name: updatedTaskName, out taskId);

            if (taskExists)
            {
                // Create a new Task object with updated details
                EFModeling.EntityProperties.DataAnnotations.Annotations.Task updatedTask = new EFModeling.EntityProperties.DataAnnotations.Annotations.Task
                {
                    TaskId = taskId,
                    Name = updatedTaskName,
                    Description = updatedTaskDesc,
                    Deadline = deadline
                };

                // Update the task
                bool success = TaskManager.Instance.UpdateTask(updatedTask);
                if (success)
                {
                    //we can also move to previous template and forward it to Error TextBlock
                    MessageBox.Show("Task updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update the task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Task not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public void TasksEditTaskCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void TasksEditTaskDeleteTaskClick(object sender, RoutedEventArgs e)
        {
            // Ensure the sender is a button and fetch the taskId from its Tag property.
            if (sender is Button deleteButton && deleteButton.Tag is long taskId)
            {
                // Instantiate the TaskManager if it hasn't been done already.
                if (TaskManager.Instance == null)
                {
                    TaskManager.Instantiate();
                }

                bool success = TaskManager.Instance.DeleteTask(taskId);
                if (success)
                {
                    MessageBox.Show("Task deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to delete the task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid task ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void TasksEditTaskViewUsersAddClick(object sender, RoutedEventArgs e)
        {
            // Assume the UI has controls to get UserId and TaskId
            int userId = int.Parse(GetTemplateControl<TextBox>("UserTextBox").Text);
            int taskId = int.Parse(GetTemplateControl<TextBox>("TaskTextBox").Text);

            // Instantiate the TaskManager if it hasn't been done already.
            if (TaskManager.Instance == null)
            {
                TaskManager.Instantiate();
            }

            // Create a new TaskDetails object
            TaskDetails taskDetails = new TaskDetails
            {
                UserId = userId,
                TaskId = taskId
            };

            bool success = true;
            if (success)
            {
                MessageBox.Show("User added to task successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                TaskManager.Instance.CreateTaskDetails(taskDetails);
            }
            else
            {
                MessageBox.Show("Failed to add user to task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void TasksEditTaskViewUsersRemoveClick(object sender, RoutedEventArgs e)
        {
            // Assume the UI has controls to get UserId and TaskId
            int userId = int.Parse(GetTemplateControl<TextBox>("UserIdTextBox").Text);
            int taskId = int.Parse(GetTemplateControl<TextBox>("TaskIdTextBox").Text);

            // Instantiate the TaskManager if it hasn't been done already.
            if (TaskManager.Instance == null)
            {
                TaskManager.Instantiate();
            }

            // Create a new TaskDetails object
            TaskDetails taskDetails = new TaskDetails
            {
                UserId = userId,
                TaskId = taskId
            };
            bool success = true;
            if (success)
            {
                TaskManager.Instance.ClearTaskDetails(taskDetails);
                MessageBox.Show("User removed from task successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to remove user from task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void TasksEditTaskAddUsersConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskAddUsersConfirm");
        }

        public void TasksEditTaskAddUsersCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void TasksEditTaskDeleteUsersAddClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskDeleteUsersRemoveClick(object sender, RoutedEventArgs e)
        {

        }

        /*public void TasksEditTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
        }

        public void TasksEditTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
        //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
            SwitchBackToPreviousTemplate();
        }*/

        public void SettingsWindowReset(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Overview");
            //ResetTextInputs(); imo there should be logic to remember basic settings but I guess we will just port it with each template load
        }

        public void SettingsWindowConfirm(object sender, RoutedEventArgs e)
        {
            //imo there should be logic that saves the settings into settings.json
        }
        /*public void TaskAddTaskListAssignLeaderConfirm(object sender, RoutedEventArgs e)
        {
            //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
        }
        public void TaskAddTaskListAssignLeaderCancel(object sender, RoutedEventArgs e)
        {
            //TASK LEADERSHIP ISN'T MENTIONED IN BASE FUNCTIONAL REQUIREMENTS SO FOR NOW IT'S COMMENTED
            SwitchBackToPreviousTemplate();
        }*/
        public void AccountClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Account");
        }
        public void ProjectsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Projects");
        }
        public void PrivacyClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Privacy");
        }
        public void DevicesClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Devices");
        }
        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("LogOut");
        }
    }
}
>>>>>>> main
