using MailSender.Models;

namespace MailSender.Interfaces
{
    public interface IEMailService
    {
        void SendMail(EMailMessage emailMessage);
    }
}