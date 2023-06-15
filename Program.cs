using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
namespace QuoteMonitor;

class Program
{
    public static MonitorInput? input;
    public static SmtpClient? smtpClient;
    public static Subject? alertSubject;
    public static string[]? emails;

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

            alertSubject = new Subject();
            emails = config.GetSection("target-emails").Get<string[]>();//config.GetSection("target-emails").Get<string[]>();

            if (emails == null || emails!.Length == 0) throw new Exception("No target e-mails attached at appsettings.json");
            foreach (var email in emails!)
            {
                EmailObserver obs = new EmailObserver(email, smtpClient);
                alertSubject.Attach(obs);
            }

            // alertSubject.CreateMsg(new AlertMessage(ePriceState.maxOverflow, input.assetName));
            EventHandler handler = new EventHandler(input, alertSubject, 3600000);
            Console.WriteLine("Press ENTER to stop the loop.");
            handler.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error:" + e.Message);
        }
    }
}