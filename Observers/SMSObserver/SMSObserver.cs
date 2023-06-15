namespace QuoteMonitor;

public class SMSObserver : IObserver
{
    SMSClientPool clientPool;
    string? phoneNumber;
    BalanceInfo balanceInfo;

    public SMSObserver(string PhoneNumber, SMSClientPool ClientPool) : base()
    {
        clientPool = ClientPool;
        phoneNumber = PhoneNumber;
    }

    protected override async void SendMessage(AlertMessage alert)
    {
        var smsClient = await clientPool.GetObject();
        var msg = MessageBox.GetSMSMessage(alert);
        smsClient.SendSMS(msg, phoneNumber!);
        clientPool.ReturnObject(smsClient);
    }

};

