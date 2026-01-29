using EmailService.Config;
using EmailService.DTO;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService.Services.Impl
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly TemplateLoader _templateLoader;
        private readonly SmtpConfig _smtp;

        public SmtpEmailSender(
            IOptions<SmtpConfig> options,
            TemplateLoader templateLoader)
        {
            _smtp = options.Value;
            _templateLoader = templateLoader;
        }

        public async Task SendAsync(EmailEvent email)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtp.From));
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
                _ => throw new ArgumentException("Unknown event type")
            };

            string template = _templateLoader.Load(templateFile);
            string body = Render(template, email.Data);

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _smtp.Host,
                _smtp.Port,
                SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                _smtp.Username,
                _smtp.Password
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private string Render(string template, Dictionary<string, object> data)
        {
            foreach (var kv in data)
            {
                template = template.Replace($"{{{{{kv.Key}}}}}", kv.Value?.ToString());
            }
            return template;
        }
    }
}
