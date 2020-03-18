using System.Threading.Tasks;

namespace Application.IServices.IHelpers
{
    public interface IEmailSenderHelper
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
