using Desktop.Database;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public UserSettingsList UserSettingsList { get; set; }
        public UserSettings UserSettings { get; set; }
        private string UserSettingsPath = "settings.json";

        public static App Instance { get; private set; }

        private void LoadAllSettings()
        {
            if (!File.Exists(UserSettingsPath))
            {
                UserSettingsList = new();
                UserSettingsList.UserSettings = new();

                UserSettings = new();
                UserSettings.DisplayProjects = DisplayProjects.All;
                UserSettings.SendSecurityAlerts = true;
                UserSettings.SendProjectAlerts = true;
                UserSettings.Enable2FA = false;
                UserSettings.RememberMe = false;

                UserSettingsList.UserSettings.Add(UserSettings);
                return;
            }

            UserSettingsList = JsonSerializer.Deserialize<UserSettingsList>(File.ReadAllText(UserSettingsPath));

            if (UserSettingsList != null && UserSettingsList.UserSettings != null)
            {
                return;
            }

            UserSettingsList = new();
            UserSettingsList.UserSettings = new();

            UserSettings = new();
            UserSettings.DisplayProjects = DisplayProjects.All;
            UserSettings.SendSecurityAlerts = true;
            UserSettings.SendProjectAlerts = true;
            UserSettings.Enable2FA = false;
            UserSettings.RememberMe = false;

            UserSettingsList.UserSettings.Add(UserSettings);
        }

        public void LoadUserSettings(User user)
        {
            UserSettings = UserSettingsList.UserSettings.Where(x => x.UserData != null && x.UserData.UserId == user.UserId).FirstOrDefault();

            if (UserSettings != null)
            {
                if (UserSettings.RememberMe)
                {
                    UserManager.Instance.CurrentSessionUser = new(UserSettings.UserData);
                }
            }
            else
            {
                UserSettings = new();
                UserSettings.UserData = new(user);
                UserSettings.DisplayProjects = DisplayProjects.All;
                UserSettings.SendSecurityAlerts = true;
                UserSettings.SendProjectAlerts = true;
                UserSettings.RememberMe = false;
                UserSettings.Enable2FA = false;

                UserSettingsList.UserSettings.Add(UserSettings);
            }
        }

        public void SaveUserSettings()
        {
            foreach (UserSettings settings in UserSettingsList.UserSettings)
            {
                if (settings.UserData != null)
                {
                    bool success = UserManager.Instance.UpdateUser(settings.UserData);

                    if (!success)
                    {
                        MessageBox.Show("There was an unexpected error!");
                        continue;
                    }
                }
            }

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            File.WriteAllText(UserSettingsPath, JsonSerializer.Serialize(UserSettingsList, options));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Security.Initialize();
            EmailService.Initialize();
            DatabaseAccess.Instantiate();
            UserManager.Instantiate();
            ProjectManager.Instantiate();
            TaskListManager.Instantiate();
            TaskManager.Instantiate();
            MeetingManager.Instantiate();
            ChatManager.Instantiate();
            Instance = this;
            LoadAllSettings();
            UserSettings = UserSettingsList.UserSettings.FirstOrDefault(x => x.RememberMe);

            if (UserSettings == null)
            {
                return;
            }

            UserManager.Instance.CurrentSessionUser = new User(UserSettings.UserData);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveUserSettings();
            base.OnExit(e);
        }

        public void SetUserSettings(UserSettings settings)
        {
            UserSettings = new UserSettings(settings);
            System.Collections.Generic.IEnumerable<UserSettings> existing = UserSettingsList.UserSettings.Where(x => x.UserData != null && x.UserData.UserId == settings.UserData.UserId);
            existing.First().UserData = new(settings.UserData);
            existing.First().SendSecurityAlerts = settings.SendSecurityAlerts;
            existing.First().SendProjectAlerts = settings.SendProjectAlerts;
            existing.First().Enable2FA = settings.Enable2FA;
            existing.First().DisplayProjects = settings.DisplayProjects;

            if (settings.RememberMe)
            {
                foreach (UserSettings setting in UserSettingsList.UserSettings)
                {
                    setting.RememberMe = false;
                }
            }

            existing.First().RememberMe = settings.RememberMe;
        }

        public void ResetSettings()
        {
            UserSettings existing = UserSettingsList.UserSettings.Where(x => x.UserData != null && x.UserData.UserId == UserSettings.UserData.UserId).First();
            UserSettingsList.UserSettings.Remove(existing);
            SaveUserSettings();
            LoadAllSettings();

            UserSettings = UserSettingsList.UserSettings.FirstOrDefault();
        }
    }
}
