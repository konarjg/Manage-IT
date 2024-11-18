using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EFModeling.EntityProperties.DataAnnotations.Annotations;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        private Regex EmailValidation = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}");
        private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");
        private Regex PhoneNumberValidation = new Regex("^{4,15}");

        private void SwitchPageTemplate(string name)
        {
            Template = Resources[name] as ControlTemplate;
        }

        private T GetTemplateControl<T>(string name) where T : class
        {
            return Template.FindName(name, this as FrameworkElement) as T;
        }

        private void SubmitRegisterForm(out User data)
        {
            var email = GetTemplateControl<TextBox>("Email").Text;
            var username = GetTemplateControl<TextBox>("Username").Text;
            var phoneNumber = GetTemplateControl<TextBox>("PhoneNumber").Text;
            var password = GetTemplateControl<PasswordBox>("Password").Password;
            var confirmPassword = GetTemplateControl<PasswordBox>("ConfirmPassword").Password;
            short prefixId = 48;

            if (email == string.Empty || username == string.Empty || phoneNumber == string.Empty
                || password == string.Empty || confirmPassword == string.Empty)
            {
                throw new Exception("You have to fill in every field!");
            }

            if (password != confirmPassword)
            {
                throw new Exception("Passwords aren't identical!");
            }

            if (!EmailValidation.IsMatch(email))
            {
                throw new Exception("Provided email is invalid!");
            }

            if (PasswordValidation.IsMatch(password))
            {
                throw new Exception("Password must be at least 8 characters long, contain at least 1 special character, at least 1 uppercase letter and at least 1 number!");
            }

            if (!PhoneNumberValidation.IsMatch(phoneNumber))
            {
                throw new Exception("Provided phone number is invalid!");
            }

            phoneNumber = Security.EncryptText(phoneNumber);
            password = Security.HashText(password, Encoding.ASCII);

            data = new();
            data.Email = email;
            data.Login = username;
            data.PhoneNumber = phoneNumber;
            data.PrefixId = prefixId;
            data.Password = password;
        }

        public MainWindow()
        {
            InitializeComponent();
            SwitchPageTemplate("Main");
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Register");
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {

        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Main");
        }

        private void SubmitRegisterFormClick(object sender, RoutedEventArgs e) 
        {
            User data;

            try
            {
                var error = string.Empty;
                SubmitRegisterForm(out data);
                RegisterController.SubmitRegisterForm(data, out error);

                GetTemplateControl<TextBlock>("Error").Text = error;
            }
            catch (Exception error)
            {
                GetTemplateControl<TextBlock>("Error").Text = error.Message;
            }
        }

    }
}
