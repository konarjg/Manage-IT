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
    /// <summary>
    /// Logika interakcji dla klasy UserSettingsWindow.xaml
    /// </summary>
    public partial class UserSettingsWindow : Window
    {
        public UserSettingsWindow()
        {
            InitializeComponent();
        }

        private void SwitchPageTemplate(string name)
        {
            Template = Resources[name] as ControlTemplate;
        }

        private T GetTemplateControl<T>(string name) where T : class
        {
            return Template.FindName(name, this as FrameworkElement) as T;
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {
            var window = new ProjectManagementWindow();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
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

        public void AppearanceClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Appearance");
        }

        public void AdditionalSettingsClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("AdditionalSettings");
        }

        public void SupportClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Support");
        }

        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }

        public void ChangeEmail(object sender, RoutedEventArgs e)
        {

        }

        public void DeleteAccountSeq(object sender, RoutedEventArgs e)
        {

        }

        public void DisableAccountSeq(object sender, RoutedEventArgs e)
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
