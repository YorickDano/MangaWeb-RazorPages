using System.Net;
using System.Net.Mail;

namespace MangaWeb.Managers
{
    class MailManager
    {
        private readonly string BaseEmail = "WhatDoYouWant?";
        private readonly string BasePassword = "WhatDoYouWant?";

        private SmtpClient SmtpClient;

        public MailManager()
        {
            SmtpClient = new SmtpClient("smtp.yandex.com.tr", 587);
            SmtpClient.UseDefaultCredentials = false;
            SmtpClient.Credentials = new NetworkCredential(BaseEmail,BasePassword);
            SmtpClient.EnableSsl = true;
        }

        public void SendMailOnRegestration(string toMail,string code)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(BaseEmail);
            mailMessage.To.Add(toMail);
            mailMessage.Body = "Hello Idiot, you trying to regestre on my site, I allow you to do this, here is your code:\n"+code;
            mailMessage.Subject = "Regestration";
            SmtpClient.Send(mailMessage);
        }
    }
}
