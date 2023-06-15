using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
namespace QuoteMonitor;

class Program
{
    static MonitorInput? input;
    static int poolEmailClientLimit = 5;
    static int poolSMSClientLimit = 5;
    static Subject alertSubject = new();
    static SmtpConfig? smtpConfig;
    static SMSConfig? smsConfig;
    static string[] emails = Array.Empty<string>();
    static string[] phoneNumbers = Array.Empty<string>();
    static CancellationTokenSource source = new();
    static EmailClientPool? emailClientPool = null;
    static SMSClientPool? smsClientPool = null;
    public static async Task Main(string[] args)
    {
        try
        {
            SetConfiguration(args);


            Console.WriteLine("Press any key to stop monitoring...");
            // alertSubject.CreateMsg(new AlertMessage(ePriceState.maxOverflow, input.assetName));
            EventHandler handler = new EventHandler(input!, alertSubject, source.Token, 1000);
            await Task.Factory.StartNew(handler.Start, source.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            Console.ReadKey();
            source.Cancel();

            if (smsClientPool != null) smsClientPool!.Dispose();
            if (emailClientPool != null) emailClientPool!.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error:" + e.Message);
        }
    }

    public static void SetConfiguration(string[] args)
    {
        input = MonitorInput.handleInput(args);

        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
        var config = builder.Build();

        SetEmailConfig(config);
        SetSMSConfig(config);
    }

    public static void SetEmailConfig(IConfigurationRoot config)
    {
        smtpConfig = new(config["email:smtp:host"]!, int.Parse(config["email:smtp:port"]!), config["email:smtp:username"]!, config["email:smtp:password"]!);
        emailClientPool = new(smtpConfig, poolEmailClientLimit);

        emails = config.GetSection("email:target-emails").Get<string[]>()!;
        if (emails == null || emails!.Length == 0) throw new Exception("No target e-mails attached at appsettings.json");
        foreach (var email in emails!)
        {
            EmailObserver obs = new(email, emailClientPool);
            alertSubject.Attach(obs);
        }
    }

    public static void SetSMSConfig(IConfigurationRoot config)
    {
        smsConfig = new(config["sms:api:api-key"]!, config["sms:api:api-secret"]!);
        smsClientPool = new(smsConfig, poolSMSClientLimit);

        phoneNumbers = config.GetSection("sms:target-phone-numbers").Get<string[]>()!;
        if (phoneNumbers == null || phoneNumbers!.Length == 0) throw new Exception("No target phone numbers attached at appsettings.json");
        foreach (var number in phoneNumbers)
        {
            SMSObserver obs = new(number, smsClientPool);
            alertSubject.Attach(obs);
        }
    }
}