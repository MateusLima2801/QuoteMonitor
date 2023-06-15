using System.Net.Mail;
namespace QuoteMonitor;

public class EmailObserver : IObserver
{
    private EmailClientPool clientPool;
    string? email;


    public EmailObserver(string Email, EmailClientPool ClientPool)
    {
        clientPool = ClientPool;
        email = Email;
    }

    protected override async void SendMessage(AlertMessage alert)
    {
        var emailClient = await clientPool.GetObject();
        var msg = MessageBox.GetMailMessage(alert, new MailAddress(emailClient.config.username));
        emailClient.SendEmail(msg, email!);
        clientPool.ReturnObject(emailClient);
    }
};

