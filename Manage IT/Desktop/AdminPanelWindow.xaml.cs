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



namespace Desktop
{
    /// <summary>
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {
        private Stack<ControlTemplate> templateHistory;
        Project project;
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
            header.Text = "Project's name";
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
                MessageBox.Show("No previous template to switch back to.");
            }
            
        }


        private void ResetTextInputs() {
            var allTextBoxes = FindVisualChildren<TextBox>(this);
            var allPasswordBoxes = FindVisualChildren<PasswordBox>(this);
            foreach (var textBox in allTextBoxes) {
                textBox.Text = string.Empty;
            }
            foreach (var passwordBox in allPasswordBoxes)
            {
                passwordBox.Clear();
            }
        }
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject { if (depObj != null) { for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) { DependencyObject child = VisualTreeHelper.GetChild(depObj, i); if (child != null && child is T) { yield return (T)child; } foreach (T childOfChild in FindVisualChildren<T>(child)) { yield return childOfChild; } } } }


        public AdminPanelWindow()
        {
            InitializeComponent();
            //add logic to lead picked project name
            templateHistory = new Stack<ControlTemplate>();
        }
      
            
        public void BackClick(object sender, RoutedEventArgs e)
        {
            //prolly if in main adm panel window then do this.close()
            SwitchBackToPreviousTemplate();
        }

        public void OverviewClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Overview");
        }

        public void UsersClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Users");
        }

        public void SupportClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Support");
        }
        public void TasksClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Tasks");
        }

        public void SecurityClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Security");
        }

        public void AuditLogClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("AuditLog");
        }

        public void InvitesClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Invites");
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
            if (Error != null && successUE) { 
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
                    bool success = UserManager.Instance.GetUserById(project.ManagerId,out User manager);
                    if (success)
                    {

                    
                        if(UserManager.Instance.CurrentSessionUser.Admin != false)
    

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
                taskListErr.Text = "Insert project's name";
            }
            else
            {
                taskListErr.Text = "An error has occured";
            }
        }

        public void TasksAddTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }

        public void TasksAddTaskListLeaderClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskListAssignLeader");
        }

        /*public void TasksAddTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }*/

        /*public void TasksAddTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();
        }*/

        public void TasksEditTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
        }

        /*public void TasksEditTaskListLeaderClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskViewUsers");
        }*/

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
            if (isDeadlineValid && taskName!="")
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
            else if(taskName==String.Empty)
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

        public void TasksAddTaskNewTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskNewTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            ResetTextInputs();
            SwitchBackToPreviousTemplate();

        }

        public void TasksAddTaskAddUsersConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskAddUsers");
        }

        public void TasksAddTaskAddUsersCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void TasksAddTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksEditTaskChangeLeader");
        }

        public void TasksAddTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void TasksEditTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void TasksEditTaskDeleteTaskClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskViewUsersAddClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskViewUsersRemoveClick(object sender, RoutedEventArgs e)
        {

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

        public void TasksEditTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }

        public void SettingsWindowReset(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Overview");
            //ResetTextInputs(); imo there should be logic to remember basic settings but I guess we will just port it with each template load
        }

        public void SettingsWindowConfirm(object sender, RoutedEventArgs e)
        {
            //imo there should be logic that saves the settings into settings.json
        }
        public void TaskAddTaskListAssignLeaderConfirm(object sender, RoutedEventArgs e)
        {

        }
        public void TaskAddTaskListAssignLeaderCancel(object sender, RoutedEventArgs e)
        {
            SwitchBackToPreviousTemplate();
        }
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

﻿using System;
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
    /// <summary>
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {
        public AdminPanelWindow()
        {
            InitializeComponent();
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {

        }

        public void OverviewClick(object sender, RoutedEventArgs e)
        {

        }

        public void UsersClick(object sender, RoutedEventArgs e)
        {

        }

        public void SupportClick(object sender, RoutedEventArgs e)
        {

        }

        public void SecurityClick(object sender, RoutedEventArgs e)
        {

        }

        public void AuditLogClick(object sender, RoutedEventArgs e)
        {

        }

        public void InvitesClick(object sender, RoutedEventArgs e)
        {

        }

        public void AdditionalSettingsClick(object sender, RoutedEventArgs e)
        {

        }

        public void DeleteProjectClick(object sender, RoutedEventArgs e)
        {

        }

        public void OverviewCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void OverviewConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UsersAddUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void UsersAddUserConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UsersAddUserCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserPageAddNewTaskToUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserPageAddExistingTaskToUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void EditUserPermsClick(object sender, RoutedEventArgs e)
        {

        }

        public void EditUserTasksClick(object sender, RoutedEventArgs e)
        {

        }

        public void DeleteUserFromProjectClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserPageCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserPageConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserDeleteConfirmCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserDeleteConfirmConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAssignMoreUsersClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAssignToExistingTaskListClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAssignToNewTaskListClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAddAnotherUserCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskAddAnotherUserConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListLeaderClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskExistingTaskListCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskExistingTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserAssignExistingTaskCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserAssignExistingTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsManagementLevelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsEditManageRightsClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTransferProjectCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTransferProjectConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsManagementLevelManagerClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsManagementLevelTasklistLeaderClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsManagementLevelTasklLeaderClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsManagementLevelTransferProjectRightsClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsManagementLevelCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsManagementLevelConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTasksConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTasksCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserTasksRemoveTaskClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserTasksNewTaskClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserTasksRemoveTasksConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserTasksRemoveTasksCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksNewTaskListClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksNewTaskClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskListCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskListLeaderClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListLeaderClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListUserListClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListDeleteClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListChangeLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListChangeLeaderCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksViewTaskListUsersAddUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAssignTaskListNewClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAssignTaskListExistingClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskNewTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskNewTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskNewTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAddUsersConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAddUsersCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskDeleteTaskClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskViewUsersAddClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskViewUsersRemoveClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskAddUsersConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskAddUsersCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskDeleteUsersAddClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskDeleteUsersRemoveClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {

        }

        public void SettingsWindowReset(object sender, RoutedEventArgs e)
        {

        }

        public void SettingsWindowConfirm(object sender, RoutedEventArgs e)
        {

        }
    }
}
