using System.Net.Mail;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace QuoteMonitor;

public class EmailClientPool : IDisposable
{
    private BlockingCollection<EmailClient> emailClientPool;
    private SemaphoreSlim semaphore;
    public EmailClientPool(SmtpConfig config, int poolSize)
    {
        emailClientPool = new BlockingCollection<EmailClient>(poolSize);
        semaphore = new SemaphoreSlim(poolSize);

        // Create and populate the pool with SmtpClient instances
        for (int i = 0; i < poolSize; i++)
        {
            emailClientPool.Add(new EmailClient(config));
        }
    }

    public async Task<EmailClient> GetClient()
    {
        await semaphore.WaitAsync();
        return emailClientPool.Take();
    }

    public void ReturnClient(EmailClient emailClient)
    {
        emailClientPool.Add(emailClient);
        semaphore.Release();
    }

    public void Dispose()
    {
        emailClientPool.CompleteAdding();

        foreach (var emailClient in emailClientPool)
        {
            emailClient.Dispose();
        }

        emailClientPool.Dispose();
    }
}

public class EmailClient
{

    SmtpClient smtpClient;

    public EmailClient(SmtpConfig config)
    {
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
