using System.Net;
using System.Net.Mail;

namespace Web
{
    public static class EmailService
    {
        private static readonly string Host = "smtp.gmail.com";
        private static readonly int Port = 587;
        private static string Email;
        private static string Password;

        public static string Parameters
        {
            get
            {
                return Security.EncryptText(Email) + "\n" + Security.EncryptText(Password);
            }
        }

        public static void Initialize()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "/smtp.cfg";

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                Email = lines[0];
                Password = lines[1];
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
            catch (Exception)
            {
                error = "There was an error while sending an email!";
                client.Dispose();
                return false;
            }
        }
    }
}
