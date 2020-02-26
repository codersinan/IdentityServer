using System;
using System.Linq;
using System.Security.Authentication;
using MailKit.Net.Smtp;
using MailSender.Interfaces;
using MailSender.Models;
using MimeKit;
using MimeKit.Text;

namespace MailSender
{
    public class CustomMailService:IEMailService
    {
        private readonly IMailConfiguration _configuration;

        public CustomMailService(IMailConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendMail(EMailMessage emailMessage)
        {
            try
            {
                var message=new MimeMessage();
                if (emailMessage.FromAddresses.Count == 0)
                {
                    emailMessage.FromAddresses.Add(new EMailAddress{Name = _configuration.DefaultSenderName,Address = _configuration.DefaultSenderAddress});
                }
                
                message.To.AddRange(emailMessage.ToAddresses.Select(x=>new MailboxAddress(x.Name,x.Address)));
                message.From.AddRange(emailMessage.FromAddresses.Select(x=>new MailboxAddress(x.Name,x.Address)));

                message.Subject = emailMessage.Subject;
                message.Body=new TextPart(TextFormat.Html)
                {
                    Text = emailMessage.Content
                };

                using (var client=new SmtpClient{SslProtocols = SslProtocols.Tls})
                {
                    client.ServerCertificateValidationCallback = (o, k, c, s) => true;
                    client.Connect(_configuration.SmtpServer,_configuration.SmtpPort,false);
                    
                    client.Authenticate(_configuration.SmtpUsername,_configuration.SmtpPassword);
                    
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}