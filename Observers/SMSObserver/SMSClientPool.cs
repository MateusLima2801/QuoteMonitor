using Vonage;
using Vonage.Request;

namespace QuoteMonitor;

public class SMSClientPool : IObjectPool<SMSClient, SMSConfig>
{
    public SMSClientPool(SMSConfig config, int poolSize) : base(config, poolSize)
    { }

    public override SMSClient Initializer(SMSConfig config)
    {
        return new SMSClient(config);
    }
}

public class SMSClient
{

    VonageClient vonageClient;

    public SMSClient(SMSConfig config)
    {
        var credentials = Credentials.FromApiKeyAndSecret(config.apiKey, config.apiSecret);
        vonageClient = new VonageClient(credentials);
    }

    public void SendSMS(string SMSMessage, string recipientPhoneNumber, string senderName = "QUOTE MONITOR REPORT")
    {
        var response = vonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
        {
            To = recipientPhoneNumber,
            From = senderName,
            Text = SMSMessage
        });

    }

    public void Dispose()
    {
        if (vonageClient is IDisposable disp)
            disp.Dispose();
    }
}
