using Microsoft.Extensions.Configuration;
namespace QuoteMonitor;

public class EmailHandler
{
    int poolEmailClientLimit;
    public List<EmailObserver> observers { get; private set; }
    SmtpConfig? smtpConfig;
    string[] emails = Array.Empty<string>();
    EmailClientPool? emailClientPool = null;

    private EmailHandler(List<EmailObserver> Observers, SmtpConfig? SmtpConfig, string[] Emails,
                        EmailClientPool? EmailClientPool, int PoolEmailClientLimit)
    {
        observers = Observers;
        smtpConfig = SmtpConfig;
        emails = Emails;
        poolEmailClientLimit = PoolEmailClientLimit;
        emailClientPool = EmailClientPool;
    }

    public static EmailHandler SetEmailConfig(IConfigurationSection config, int limit = 5)
    {
        var host = config.GetSection("smtp:host").Get<string>();
        var port = config.GetSection("smtp:port").Get<int>();
        var username = config.GetSection("smtp:username").Get<string>();
        var password = config.GetSection("smtp:password").Get<string>();
        var smtpConfig = new SmtpConfig(host!, port, username!, password!);
        var emailClientPool = new EmailClientPool(smtpConfig, limit);

        var emails = config.GetSection("target-emails").Get<string[]>()!;
        if (emails == null) throw new Exception("No target e-mails attached at appsettings.json");
        List<EmailObserver> observers = new();
        foreach (var email in emails!)
        {
            observers.Add(new EmailObserver(email, emailClientPool));
        }
        return new EmailHandler(observers, smtpConfig, emails, emailClientPool, limit);
    }

    public void Dispose()
    {
        if (emailClientPool != null) emailClientPool!.Dispose();
    }
}