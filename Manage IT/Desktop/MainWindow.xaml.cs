using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
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
using System.Diagnostics.Eventing.Reader;
//using EFModeling.EntityPropertiesBase.DataAnnotations.Annotations;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        private Regex EmailValidation = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}");
        private Regex PasswordValidation = new Regex("^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");

        public void SwitchPageTemplate(string name)
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
            var password = GetTemplateControl<PasswordBox>("Password").Password;
            var confirmPassword = GetTemplateControl<PasswordBox>("ConfirmPassword").Password;

            if (email == string.Empty || username == string.Empty
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

            password = Security.HashText(password, Encoding.UTF8);

            data = new();
            data.Email = email;
            data.Login = username;
            data.Password = password;
        }
        private void SubmitForgotPasswordForm(out User data)
        {
            var credential = GetTemplateControl<TextBox>("Credential").Text;
            var password = GetTemplateControl<PasswordBox>("Password").Password;
            var confirmPassword = GetTemplateControl<PasswordBox>("ConfirmPassword").Password;
            

            if (credential == string.Empty || password == string.Empty || confirmPassword == string.Empty)
            {
                throw new Exception("You have to fill in every field!");
            }

            if (password != confirmPassword)
            {
                throw new Exception("Passwords aren't identical!");
            }
            /*
            if (!EmailValidation.IsMatch(credential))
            {
                throw new Exception("Provided email is invalid!");
            }*/

            if (PasswordValidation.IsMatch(password))
            {
                throw new Exception("Password must be at least 8 characters long, contain at least 1 special character, at least 1 uppercase letter and at least 1 number!");
            }

            password = Security.HashText(password, Encoding.UTF8);
            data = new();
            data.Login = credential;
            data.Email = credential;
            data.Password = password;
        }



        private void SubmitLoginForm(out User data)
        {
            var credential = GetTemplateControl<TextBox>("Credential").Text;
            var password = GetTemplateControl<PasswordBox>("Password").Password;

            if (credential == string.Empty || password == string.Empty)
            {
                throw new Exception("You have to fill in every field!");
            }

            password = Security.HashText(password, Encoding.UTF8);

            data = new();
            data.Email = credential;
            data.Login = credential;
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
            SwitchPageTemplate("Login");
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Main");
        }

        //TEST BUTTONS (to "summon" these windows)
        private void CalendarTestClick(object sender, RoutedEventArgs e)
        {
            CalendarWindow selectedFileWindow = new CalendarWindow(); selectedFileWindow.Show();
        }
        private void AdminPanelTestClick(object sender, RoutedEventArgs e)
        {
            AdminPanelWindow selectedFileWindow = new AdminPanelWindow(); selectedFileWindow.Show();
        }


        public void ForgotPasswordClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("ForgotPassword");
        }

        public void ForgotPasswordBackClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Login");
        }

        private void SubmitRegisterFormClick(object sender, RoutedEventArgs e) 
        {
            User data;
            GetTemplateControl<TextBlock>("Error").Foreground = Brushes.Red;

            try
            {
                var error = string.Empty;
                SubmitRegisterForm(out data);
                RegisterController.SubmitRegisterForm(data, out error);

                if (error == string.Empty)
                {
                    GetTemplateControl<TextBlock>("Error").Foreground = Brushes.White;
                    GetTemplateControl<TextBlock>("Error").Text = "Verify your email address!";
                    return;
                }

                GetTemplateControl<TextBlock>("Error").Text = error;
            }
            catch (Exception error)
            {
                GetTemplateControl<TextBlock>("Error").Text = error.Message;
            }
        }

        public void SubmitLoginFormClick(object sender, RoutedEventArgs e)
        {
            User data;

            try
            {
                var error = string.Empty;
                SubmitLoginForm(out data);
                LoginController.SubmitLoginForm(data, out error);
                GetTemplateControl<TextBlock>("Error").Text = error;
            }
            catch (Exception error)
            {
                GetTemplateControl<TextBlock>("Error").Text = error.Message;
            }
        }

        private void SubmitForgotPasswordFormClick(object sender, RoutedEventArgs e)
        {
            User data;
            GetTemplateControl<TextBlock>("Error").Foreground = Brushes.Red;

            try
            {
                var error = string.Empty;
                SubmitForgotPasswordForm(out data);
                ForgotPasswordController.SubmitForgotPasswordForm(data, out error);

                if (error == string.Empty)
                {
                    GetTemplateControl<TextBlock>("Error").Foreground = Brushes.White;
                    GetTemplateControl<TextBlock>("Error").Text = "Password restoration email has been sent";
                    return;
                }

                GetTemplateControl<TextBlock>("Error").Text = error;
            }
            catch (Exception error)
            {
                GetTemplateControl<TextBlock>("Error").Foreground = Brushes.Red;
                GetTemplateControl<TextBlock>("Error").Text = error.Message;
            }
        }
        /*
        public void SubmitForgotPasswordFormClick(object sender, RoutedEventArgs e)
        {
            User data;
            GetTemplateControl<TextBlock>("Error").Foreground = Brushes.Red;

            try
            {
                var error = string.Empty;
                SubmitForgotPasswordForm(out data);
                ForgotPasswordController.SubmitForgotPasswordForm(data, out error);

                if (error == string.Empty)
                {
                    GetTemplateControl<TextBlock>("Error").Foreground = Brushes.White;
                    GetTemplateControl<TextBlock>("Error").Text = error;
                    //Console.Write("TEST FORGOTA");
                    return;
                }
                
                GetTemplateControl<TextBlock>("Error").Text = error;
            }
            catch (Exception error)
            {
                //Console.Write("TEST FORGOTA WYJ");
                GetTemplateControl<TextBlock>("Error").Text = error.Message;
            }
        }
        */
    }
}
