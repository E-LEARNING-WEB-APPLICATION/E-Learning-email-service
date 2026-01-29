
using EmailService.DTO;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmailService.Services.Impl
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly TemplateLoader _templateLoader;

        public SmtpEmailSender(IConfiguration config, TemplateLoader templateLoader)
        {
            _config = config;
            _templateLoader = templateLoader;
        }
        public async Task SendAsync(EmailEvent email)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Smtp:From"]));
            email.To.ForEach(e => message.To.Add(MailboxAddress.Parse(e)));
            message.Subject = email.Subject;

            string templateFile = email.EventType switch
            {
                "SIGN_IN_OTP" => "sign-in-otp.html",
                "COURSE_PURCHASED" => "course-purchased.html",
                "INSTRUCTOR_APPROVED" => "instructor-approved.html",
                "INSTRUCTOR_APPROVAL_PENDING" => "instructor-approval-pending.html",
                "INSTRUCTOR_PAYOUT" => "instructor-payout.html",
                "PASSWORD_CHANGED" => "password-changed.html",
                "PAYMENT_SUCCESS" => "payment-success.html",
                "PAYMENT_FAILED" => "payment-failed.html",
                _ => throw new Exception("Unknown event type")
            };

            string template = _templateLoader.Load(templateFile);
            string body = Render(template, email.Data);

            message.Body = new TextPart("html") { Text = body };
            using var client = new SmtpClient();
            await client.ConnectAsync(
                _config["Smtp:Host"],
                int.Parse(_config["Smtp:Port"]),
                SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                _config["Smtp:Username"],
                _config["Smtp:Password"]
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        string Render(string template, Dictionary<string, object> data)
        {
            foreach (var key in data)
            {
                template = template.Replace($"{{{{{key.Key}}}}}", key.Value.ToString());
            }
            return template;
        }
    }
}
