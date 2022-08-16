using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace EmptyAPI.Services
{
    public class EmailService
    {
        private readonly string _privateEmail;
        private readonly string _privatePassword;

        public EmailService(string privateEmail, string privatePassword)
        {
            _privateEmail = privateEmail;
            _privatePassword = privatePassword;
        }


        public bool SendEmail(string UserEmail, string subject, string content, byte[] bytes = null, string filename = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_privateEmail);
            mailMessage.To.Add(new MailAddress(UserEmail));

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = content;
            mailMessage.Attachments.Add(new Attachment(new MemoryStream(bytes), filename));

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(_privateEmail, _privatePassword);
            client.Host = "smtp.mail.ru";
            client.EnableSsl = true;
            client.Port = 587;

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (System.Exception)
            {


            }

            return false;
        }


    }
}
