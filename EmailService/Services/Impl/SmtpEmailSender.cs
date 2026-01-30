using EmailService.Config;
using EmailService.DTO;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService.Services.Impl
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly TemplateLoader _templateLoader;
        private readonly SmtpConfig _smtp;
        private readonly HttpClient _httpClient;

        private const int MAX_ATTACHMENT_SIZE = 10 * 1024 * 1024; // 10 MB

        public SmtpEmailSender(
            IOptions<SmtpConfig> options,
            TemplateLoader templateLoader,
            HttpClient httpClient)
        {
            _smtp = options.Value;
            _templateLoader = templateLoader;
            _httpClient = httpClient;
        }

        public async Task SendAsync(EmailEvent email)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtp.From));
            email.To.ForEach(t => message.To.Add(MailboxAddress.Parse(t)));
            message.Subject = email.Subject;

            var template = _templateLoader.LoadByEventType(email.EventType);
            var htmlBody = Render(template, email.Data);

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            if (email.Attachments?.Any() == true)
            {
                await AddAttachmentsAsync(bodyBuilder, email.Attachments);
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private async Task AddAttachmentsAsync(
            BodyBuilder builder,
            Dictionary<string, string> attachments)
        {
            var provider = new FileExtensionContentTypeProvider();

            var tasks = attachments.Select(async kv =>
            {
                var displayName = kv.Key;
                var url = kv.Value;

                var bytes = await _httpClient.GetByteArrayAsync(url);

                if (bytes.Length > MAX_ATTACHMENT_SIZE)
                    throw new Exception($"Attachment too large: {displayName}");

                if (!provider.TryGetContentType(displayName, out var contentType))
                    contentType = "application/octet-stream";

                return (displayName, bytes, contentType);
            });

            var files = await Task.WhenAll(tasks);

            foreach (var f in files)
            {
                builder.Attachments.Add(
                    f.displayName,
                    f.bytes,
                    ContentType.Parse(f.contentType)
                );
            }
        }

        private string Render(string template, Dictionary<string, object> data)
        {
            foreach (var kv in data)
            {
                template = template.Replace(
                    $"{{{{{kv.Key}}}}}",
                    kv.Value?.ToString() ?? string.Empty
                );
            }
            return template;
        }
    }
}