namespace MailSender.Interfaces
{
    public interface IMailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }

        public string DefaultSenderName { get; set; }
        public string DefaultSenderAddress { get; set; }
    }
}