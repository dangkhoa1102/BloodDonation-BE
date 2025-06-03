using System.Threading.Tasks;

namespace Blood_Donation_System.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
} 