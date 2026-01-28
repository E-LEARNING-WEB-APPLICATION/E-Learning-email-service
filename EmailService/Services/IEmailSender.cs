using EmailService.DTO;

namespace EmailService.Services
{
    public interface IEmailSender
    {
        Task SendAsync(EmailEvent request);
    }
}
