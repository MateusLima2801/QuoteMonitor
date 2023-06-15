using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
namespace QuoteMonitor;

class Program
{
    public static MonitorInput? input;
    public static SmtpClient? smtpClient;
    public static Subject alertSubject = new();
    public static string[]? emails;
    public static CancellationTokenSource source = new();
    public static async Task Main(string[] args)
    {
        try
        {
            input = MonitorInput.handleInput(args);

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();

            smtpClient = new SmtpClient(config["smtp:host"])
            {
                Port = int.Parse(config["smtp:port"]!),
                Credentials = new NetworkCredential(config["smtp:username"], config["smtp:password"]),
                EnableSsl = true,
            };

            emails = config.GetSection("target-emails").Get<string[]>();//config.GetSection("target-emails").Get<string[]>();

            if (emails == null || emails!.Length == 0) throw new Exception("No target e-mails attached at appsettings.json");
            foreach (var email in emails!)
            {
                EmailObserver obs = new EmailObserver(email, smtpClient);
                alertSubject.Attach(obs);
            }

            // alertSubject.CreateMsg(new AlertMessage(ePriceState.maxOverflow, input.assetName));
            EventHandler handler = new EventHandler(input, alertSubject, source.Token/*, 3600000*/);
            await Task.Factory.StartNew(handler.Start, TaskCreationOptions.LongRunning);

            Console.ReadKey();

            source.Cancel();
        }
        //catch (TaskCanceledException) { }
        catch (Exception e)
        {
            Console.WriteLine("Error:" + e.Message);
        }
    }
}