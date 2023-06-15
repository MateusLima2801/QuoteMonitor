using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
namespace QuoteMonitor;

class Program
{
    static MonitorInput? input;
    static int poolEmailClientLimit = 5;
    static Subject alertSubject = new();
    static SmtpConfig? smtpConfig;
    static string[] emails = Array.Empty<string>();
    static CancellationTokenSource source = new();
    static EmailClientPool? emailClientPool;
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

        smtpConfig = new(config["smtp:host"]!, int.Parse(config["smtp:port"]!), config["smtp:username"]!, config["smtp:password"]!);
        emailClientPool = new(smtpConfig, poolEmailClientLimit);

        emails = config.GetSection("target-emails").Get<string[]>()!;
        if (emails == null || emails!.Length == 0) throw new Exception("No target e-mails attached at appsettings.json");
        foreach (var email in emails!)
        {
            EmailObserver obs = new EmailObserver(email, emailClientPool);
            alertSubject.Attach(obs);
        }

    }
}