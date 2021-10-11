using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IEmailService
    {
        Task SendEmailConfirmationMail(string userId, string email, string token);
        Task SendForgotPasswordMail(string userId, string email, string token);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailConfirmationMail(string userId, string email, string token)
        {
            string href = _config["ApiBaseUrl"] + $"/api/auth/confirm-email?userId={userId}&token={token}";
            string content = $@"
                <p>
                    Hello, <br> Thank you for joining {_config["AppName"]}. <br>
                    Please <a href='{href}'> click here </a> to confirm your email.
                </p>";
            SendGridModel model = new()
            {
                Subject = "Email Confirmation",
                To = email,
                PlainText = "",
                HtmlContent = content
            };
            await SendEmail(model);
        }

        public async Task SendForgotPasswordMail(string userId, string email, string token)
        {
            string href = _config["ClientBaseUrl"] + $"/auth/reset-password?userId={userId}&token={token}";
            string content = $@"
                <p>
                    Please <a href='{href}'> click here </a> to reset your password.
                </p>";
            SendGridModel model = new()
            {
                Subject = "Reset Password",
                To = email,
                PlainText = "",
                HtmlContent = content
            };
            await SendEmail(model);
        }

        private async Task SendEmail(SendGridModel model)
        {
            var apiKey = _config["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config["SendGrid:From"], _config["SendGrid:Name"]);
            var to = new EmailAddress(model.To);
            var msg = MailHelper.CreateSingleEmail(from, to, model.Subject, model.PlainText, model.HtmlContent);
            await client.SendEmailAsync(msg);
        }
    }

    public class SendGridModel
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string PlainText { get; set; }
        public string HtmlContent { get; set; }
    }
}
