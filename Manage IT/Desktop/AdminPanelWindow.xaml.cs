using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;



namespace Desktop
{
    /// <summary>
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {
        int switches = 0;
        string currentTemplate = "Overview";
        string previousTemplate = "";
        private void SwitchPageTemplate(string name)
        {
            switches++;
            previousTemplate = currentTemplate;
            currentTemplate = name;
            Template = Resources[name] as ControlTemplate;
        }
        private void RevertToPreviousTemplate()
        {
            switches++;
            Template = Resources[previousTemplate] as ControlTemplate;
            currentTemplate = previousTemplate;
        }
        public AdminPanelWindow()
        {
            InitializeComponent();
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {
            //prolly if in main adm panel window then do this.close()
            SwitchPageTemplate("Overview");
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

        public void DeleteProjectClick(object sender, RoutedEventArgs e)
        {
            string error;
            //bool success = ProjectManager.DeleteProject(4);

        }

        public void OverviewCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
           
        }

        public void OverviewConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("OverviewConfirm");
        }

        public void UsersAddUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void UsersAddUserConfirmClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("UsersAddUserConfirm");
        }
        public void AppearanceClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Appearance");
        }

        public void UsersAddUserCancelClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Overview");
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
            RevertToPreviousTemplate();
        }

        public void UserPageConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserDeleteConfirmCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void UserDeleteConfirmConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
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
            RevertToPreviousTemplate();
        }

        public void UserNewTaskAddAnotherUserConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskNewTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void UserNewTaskNewTaskListLeaderClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserNewTaskExistingTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void UserNewTaskExistingTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserAssignExistingTaskCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
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
            RevertToPreviousTemplate();
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
            RevertToPreviousTemplate();
        }

        public void UserEditPermissionsManagementLevelConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTasksConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void UserEditPermissionsTasksCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
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
            RevertToPreviousTemplate();
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
            RevertToPreviousTemplate();
        }

        public void TasksAddTaskListLeaderClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("TasksAddTaskListAssignLeader");
        }

        public void TasksAddTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void TasksEditTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskListCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
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
            SwitchPageTemplate("TasksAddTaskListChangeLeader");
        }

        public void TasksEditTaskListChangeLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void TasksViewTaskListUsersAddUserClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void TasksAddTaskAssignTaskListNewClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAssignTaskListExistingClick(object sender, RoutedEventArgs e)
        {
            // Przykładowa logika dodawania zadania do istniejącej listy
            string taskName = "Nowe zadanie"; // Możesz to zamienić na dynamicznie pobierane dane
            DateTime taskDueDate = DateTime.Now.AddDays(7); // Przykładowa data wykonania

            // Tworzenie nowego zadania
            /*Task newTask = new Task
            {
                Name = taskName,
                DueDate = taskDueDate
            };

            // Dodanie zadania do listy
            ExistingTaskList.Add(newTask);

            // Wyświetlenie komunikatu informacyjnego
            MessageBox.Show($"Zadanie '{taskName}' zostało dodane do listy.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);*/
        }


        public void TasksAddTaskNewTaskListConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskNewTaskListAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskNewTaskListAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void TasksAddTaskAddUsersConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAddUsersCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void TasksAddTaskAssignLeaderConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksAddTaskAssignLeaderCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
        }

        public void TasksEditTaskConfirmClick(object sender, RoutedEventArgs e)
        {

        }

        public void TasksEditTaskCancelClick(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
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
            RevertToPreviousTemplate();
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
            RevertToPreviousTemplate();
        }

        public void SettingsWindowReset(object sender, RoutedEventArgs e)
        {

        }

        public void SettingsWindowConfirm(object sender, RoutedEventArgs e)
        {

        }
        public void TaskAddTaskListAssignLeaderConfirm(object sender, RoutedEventArgs e)
        {

        }
        public void TaskAddTaskListAssignLeaderCancel(object sender, RoutedEventArgs e)
        {
            RevertToPreviousTemplate();
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
            /*data = new();
            data.Email = null;
            data.Login = null;
            data.Password = null;*/
            this.Close();
        }
        /*public void LogOutClick(object sender, RoutedEventArgs e)
        {

        }*/
    }
}
