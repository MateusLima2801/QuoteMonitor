using Microsoft.Extensions.Configuration;
namespace QuoteMonitor;

public class SMSHandler
{
    int poolSMSClientLimit;
    public List<SMSObserver> observers { get; private set; }
    SMSConfig? smsConfig;
    string[] phoneNumbers = Array.Empty<string>();
    SMSClientPool? smsClientPool = null;

    private SMSHandler(List<SMSObserver> Observers, SMSConfig? SmsConfig, string[] PhoneNumbers,
                        SMSClientPool? SmsClientPool, int PoolSMSClientLimit)
    {
        observers = Observers;
        smsConfig = SmsConfig;
        phoneNumbers = PhoneNumbers;
        poolSMSClientLimit = PoolSMSClientLimit;
        smsClientPool = SmsClientPool;
    }

    public static SMSHandler SetSMSConfig(IConfigurationSection config, int limit = 5)
    {
        var apiKey = config.GetSection("api:api-key").Get<string>();
        var apiSecret = config.GetSection("api:api-secret").Get<string>();
        var smsConfig = new SMSConfig(apiKey!, apiSecret!);
        var smsClientPool = new SMSClientPool(smsConfig, limit);

        var phoneNumbers = config.GetSection("target-phone-numbers").Get<string[]>()!;
        if (phoneNumbers == null) throw new Exception("No target phone numbers attached at appsettings.json");
        List<SMSObserver> observers = new();
        foreach (var number in phoneNumbers)
        {
            observers.Add(new SMSObserver(number, smsClientPool));
        }


        return new SMSHandler(observers, smsConfig, phoneNumbers, smsClientPool, limit);
    }

    public void Dispose()
    {
        if (smsClientPool != null) smsClientPool!.Dispose();
    }
}