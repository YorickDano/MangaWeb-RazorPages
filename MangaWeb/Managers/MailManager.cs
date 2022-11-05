using MangaWeb.OptionModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MangaWeb.Managers
{
    class MailManager : IEmailSender
    {
        private readonly string BaseEmail;
        private readonly string BasePassword;

        private SmtpClient SmtpClient;

        public MailManager(IOptionsSnapshot<MailSenderOptions> options)
        {
            var mailSenderOptions = options.Value;
            BaseEmail = mailSenderOptions.BaseEmail;
            BasePassword = mailSenderOptions.BasePassword;

            SmtpClient = new SmtpClient("smtp.mail.ru", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(BaseEmail, BasePassword),
                EnableSsl = true
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(BaseEmail),
                Body = htmlMessage,
                Subject = subject,
                IsBodyHtml = true

            };

            mailMessage.To.Add(email);
            await SmtpClient.SendMailAsync(mailMessage);
        }
    }
}
