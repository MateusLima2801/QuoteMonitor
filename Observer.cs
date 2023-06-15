using System.Net.Mail;
namespace QuoteMonitor;

public abstract class IObserver
{
    public abstract void Update(AlertMessage priceState);
};

public class Subject
{
    List<IObserver> EmailObservers = new List<IObserver>();
    AlertMessage? _alert;

    public void Attach(params IObserver[] obsArray)
    {
        foreach (var obs in obsArray)
        {
            EmailObservers.Add(obs);
        }

    }

    public void Detach(params IObserver[] obsArray)
    {
        foreach (var obs in obsArray)
        {
            EmailObservers.Remove(obs);
        }
    }

    private void Notify()
    {
        Parallel.ForEach(EmailObservers, obs =>
        {
            obs.Update(_alert!);
        });
    }

    public void CreateMsg(AlertMessage alert)
    {
        _alert = alert;
        //Console.WriteLine("Subject: mudei meu estado --- notificando observadores...");
        Notify();
    }
};


public class EmailObserver : IObserver
{

    private EmailClientPool clientPool;
    AlertMessage? receivedMsg;
    string? email;
    int? number;

    protected static int s_id = 1;

    public EmailObserver(string Email, EmailClientPool ClientPool)
    {
        clientPool = ClientPool;
        number = EmailObserver.s_id;
        EmailObserver.s_id++;
        email = Email;
    }

    void PrintMsg()
    {
        Console.WriteLine("Observador numero: " + number + " recebendo mensagem --- " + receivedMsg!.priceState);
    }

    async void SendEmail()
    {
        var emailClient = await clientPool.GetClient();
        var msg = MailBox.GetMailMessage(receivedMsg!);
        emailClient.SendEmail(msg, email!);
        clientPool.ReturnClient(emailClient);
    }

    public override void Update(AlertMessage alert)
    {
        receivedMsg = alert;
        //PrintMsg();
        SendEmail();
    }
};


