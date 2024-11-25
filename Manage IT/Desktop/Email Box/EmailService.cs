using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var url = "http://manageit.runasp.net/GetSmtpParameters";
       
            using (var client = new HttpClient())
            {
                var message = new HttpRequestMessage(HttpMethod.Get, url);
                var response = client.Send(message);

                using (var stream = response.Content.ReadAsStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        Email = Security.DecryptText(reader.ReadLine());
                        Password = Security.DecryptText(reader.ReadLine());
                    }
                }
            }
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
