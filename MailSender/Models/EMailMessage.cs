using System.Collections.Generic;

namespace MailSender.Models
{
    public class EMailMessage
    {
        public string Subject { get; set; }
        public string Content { get; set; }

        public List<EMailAddress> FromAddresses { get; set; }
        public List<EMailAddress> ToAddresses { get; set; }

        public EMailMessage()
        {
            FromAddresses = new List<EMailAddress>();
            ToAddresses = new List<EMailAddress>();
        }
    }
}