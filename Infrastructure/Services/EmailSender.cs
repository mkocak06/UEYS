using Core.Interfaces;
using Core.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailSender(EmailConfiguration emailConfig, IWebHostEnvironment webHostEnvironment)
        {
            _emailConfig = emailConfig;
            _webHostEnvironment = webHostEnvironment;
        }

        public void SendEmail(EmailMessage message, string templateFileName = null)
        {
            if (templateFileName != null)
            {
                var pathToFile = Path.Combine(_webHostEnvironment.WebRootPath, "HTMLTemplates", templateFileName);

                using (StreamReader sourceReader = File.OpenText(pathToFile))
                {
                    message.Body = sourceReader.ReadToEnd();
                }
            }

            SmtpClient smtpClient = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.Port);
            var networkCredential = new NetworkCredential(_emailConfig.From, _emailConfig.Password);

            CredentialCache cache = new CredentialCache(); // lokalde commentle

            smtpClient.Credentials = networkCredential;

            cache.Add(_emailConfig.SmtpServer, _emailConfig.Port, "Login",
                networkCredential); // Serverda GSS Hatası vermemesi için       // lokalde commentle

            smtpClient.Credentials = cache; // lokalde commentle
            //smtpClient.UseDefaultCredentials = false; // uncomment if you don't want to use the network credentials
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = false;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress(_emailConfig.From, _emailConfig.UserName);
            if (message.To != null)
            {
                foreach (var to in message.To)
                {
                    mail.To.Add(new MailAddress(to));
                }
            }

            if (message.Bcc != null)
            {
                foreach (var bcc in message.Bcc)
                {
                    mail.Bcc.Add(new MailAddress(bcc));
                }
            }

            if (message.CC != null)
            {
                foreach (var cc in message.CC)
                {
                    mail.CC.Add(new MailAddress(cc));
                }
            }

            mail.IsBodyHtml = true;
            mail.Subject = message.Subject;
            mail.Body = message.Body;

            if (message.AttachedFiles != null)
            {
                foreach (var attachedFile in message.AttachedFiles)
                {
                    mail.Attachments.Add(new Attachment(attachedFile.AttachmentStream, attachedFile.AttachmentTitle,
                        attachedFile.ContentType));
                }
            }

            smtpClient.Send(mail);

        }
    }
}