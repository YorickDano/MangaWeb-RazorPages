using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace MangaWeb.Managers
{
    class MailManager : IEmailSender
    {
        private readonly string BaseEmail = "WhatDoYouWant?";
        private readonly string BasePassword = "WhatDoYouWant?";

        private SmtpClient SmtpClient;

        public MailManager()
        {
            SmtpClient = new SmtpClient("smtp.mail.ru", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(BaseEmail, BasePassword),
                EnableSsl = true
            };
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(BaseEmail),
                Body = htmlMessage,
                Subject = subject,
                IsBodyHtml = true
                
            };

            mailMessage.To.Add(email);
            SmtpClient.Send(mailMessage);

            return Task.CompletedTask;
        }
    }
}
