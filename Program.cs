using Microsoft.Extensions.Configuration;
namespace QuoteMonitor;

class Program
{
    static MonitorInput? input;
    static EmailHandler? emailHandler = null;
    static SMSHandler? smsHandler = null;
    static Subject alertSubject = new();
    static CancellationTokenSource source = new();

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

            if (smsHandler != null) smsHandler!.Dispose();
            if (emailHandler != null) emailHandler!.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
        }
    }

    public static void SetConfiguration(string[] args)
    {
        input = MonitorInput.handleInput(args);

        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
        var config = builder.Build();

        emailHandler = EmailHandler.SetEmailConfig(config.GetSection("email"));
        alertSubject.Attach(emailHandler.observers.ToArray());

        smsHandler = SMSHandler.SetSMSConfig(config.GetSection("sms"));
        alertSubject.Attach(smsHandler.observers.ToArray());
    }


}