using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomLogger.Services
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(string name, string email, List<string> ccEmails, string subject, string msg);
    }
}
