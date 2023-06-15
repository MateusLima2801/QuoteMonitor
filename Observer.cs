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
        foreach (var obs in EmailObservers)
        {
            obs.Update(_alert!);
        }
    }

    public void CreateMsg(AlertMessage alert)
    {
        _alert = alert;
        Console.WriteLine("Subject: mudei meu estado --- notificando observadores...");
        Notify();
    }
};


public class EmailObserver : IObserver
{
    private EmailClientSingleton emailClient;
    AlertMessage? receivedMsg;
    string? email;
    int? number;

    protected static int s_id = 1;

    public EmailObserver(string Email, SmtpClient client)
    {
        emailClient = EmailClientSingleton.GetInstance(client);
        number = EmailObserver.s_id;
        EmailObserver.s_id++;
        email = Email;
    }

    void PrintMsg()
    {
        Console.WriteLine("Observador numero: " + number + " recebendo mensagem --- " + receivedMsg.priceState);
    }

    void SendEmail()
    {
        var msg = MailBox.GetMailMessage(receivedMsg!);
        emailClient.SendEmail(msg, email!);
    }

    public override void Update(AlertMessage alert)
    {
        receivedMsg = alert;
        PrintMsg();
        //SendEmail();
    }
};


//UNITY TEST FOR EmailObserver
// Subject sub = new();

// // criando ObservadoresConcretos
// EmailObserver obs1 = new();
// EmailObserver obs2 = new();
// EmailObserver obs3 = new();
// sub.attach(obs1, obs3);

// // 1a mudança de estado
// sub.createMsg(ePriceState.maxOverflow);

// // adicionando um novo ObservadorConcreto
// // 2a mudança de estado
// sub.attach(obs2);
// sub.createMsg(ePriceState.minOverflow);

// // removendo um ObservadorConcreto
// // 3a mudança de estado
// sub.detach(obs1);
// sub.createMsg(ePriceState.maxOverflow);

