using System.Net.Mail;
namespace QuoteMonitor;

public class EmailClientSingleton
{
    private static EmailClientSingleton? instance;
    SmtpClient smtpClient;


    private EmailClientSingleton(SmtpClient client)
    {
        smtpClient = client;
    }

    public static EmailClientSingleton GetInstance(SmtpClient client)
    {
        if (instance == null) instance = new(client);
        return instance;
    }

    public void SendEmail(MailMessage mailMessage, string recipientEmail)
    {
        mailMessage.To.Add(recipientEmail);
        smtpClient.Send(mailMessage);
    }
}
