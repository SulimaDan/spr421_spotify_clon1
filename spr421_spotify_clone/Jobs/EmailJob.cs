using Microsoft.AspNetCore.Identity;
using Quartz;
using spr421_spotify_clone.DAL.Entities.Identity;
using System.Net.Mail;

namespace spr421_spotify_clone.Jobs
{
    public class EmailJob : IJob
    {
        private readonly SmtpClient _smtpClient;
        private readonly string emailAddress;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailJob(UserManager<ApplicationUser> userManager)
        {
            emailAddress = "";
            string password = "";
            _smtpClient = new SmtpClient();
            _smtpClient.Host = "smtp.gmail.com";
            _smtpClient.Port = 587;
            _smtpClient.EnableSsl = true;
            _smtpClient.Credentials = new System.Net.NetworkCredential(emailAddress, password);
            _userManager = userManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var emails = _userManager.Users.Select(u => u.Email).ToList();

            var message = new MailMessage();
            message.From = new MailAddress(emailAddress);
            foreach (var email in emails)
            {
                message.To.Add(email);
            }
            message.Subject = "Scheduled Email from Quartz.NET Job";
            message.Body = "This is a test email sent from a Quartz.NET scheduled job.";
            await _smtpClient.SendMailAsync(message);
        }
    }
}
