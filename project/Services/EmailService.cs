using System.Threading.Tasks;

namespace project.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class MockEmailService : IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // In thực tế sẽ dùng SMTP hoặc SendGrid
            // Ở đây mình giả lập để phục vụ demo
            System.Diagnostics.Debug.WriteLine($"To: {email}, Subject: {subject}, Body: {message}");
            return Task.CompletedTask;
        }
    }
}
