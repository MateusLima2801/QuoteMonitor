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
        var smtpConfig = new SmtpConfig(config["smtp:host"]!, int.Parse(config["smtp:port"]!), config["smtp:username"]!, config["smtp:password"]!);
        var emailClientPool = new EmailClientPool(smtpConfig, limit);

        var emails = config.GetSection("target-emails").Get<string[]>()!;
        if (emails == null || emails!.Length == 0) throw new Exception("No target e-mails attached at appsettings.json");
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