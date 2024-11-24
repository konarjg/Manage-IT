using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop
{
    public static class EmailService
    {
        private static readonly string Host = "smtp.gmail.com";
        private static readonly int Port = 587;
        private static string Email;
        private static string Password; 
    
        public static void Initialize()
        {
            Email = Security.DecryptText("0G4yZ9VbqWuQ3hY0bhknXLqNczZS6xQIKJB7mGut8qA=");
            Password = Security.DecryptText("+j+Os6f3nMUlhVdESQXddMOGjg2HcUGsrew5AG2+okA=");
        }

        public static bool SendEmail(string targetEmail, string subject, string body, out string error)
        {
            MailAddress from = new(Email);
            MailAddress to = new(targetEmail);

            MailMessage message = new(from, to);
            message.IsBodyHtml = true;
            message.Subject = subject;
            message.Body = body;

            SmtpClient client = new();
            client.Host = Host;
            client.Port = Port;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(Email, Password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;

            try
            {
                client.Send(message);
                client.Dispose();
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = "There was an error while sending an email!";
                client.Dispose();
                return false;
            }
        }
    }
}
