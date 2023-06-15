using System.Net.Mail;
using System.Collections.Concurrent;
using System.Net;

namespace QuoteMonitor;

public class EmailClientPool : IObjectPool<EmailClient, SmtpConfig>
{
    public EmailClientPool(SmtpConfig config, int poolSize) : base(config, poolSize)
    { }

    public override EmailClient Initializer(SmtpConfig config)
    {
        return new EmailClient(config);
    }
}

public class EmailClient
{
    public SmtpConfig config { get; private set; }
    SmtpClient smtpClient;

    public EmailClient(SmtpConfig Config)
    {
        config = Config;
        smtpClient = new SmtpClient(config.host)
        {
            Port = config.port,
            Credentials = new NetworkCredential(config.username, config.password),
            EnableSsl = true,
        };
    }

    public void SendEmail(MailMessage mailMessage, string recipientEmail)
    {
        mailMessage.To.Add(recipientEmail);
        smtpClient.Send(mailMessage);
    }

    public void Dispose()
    {
        smtpClient.Dispose();
    }
}
