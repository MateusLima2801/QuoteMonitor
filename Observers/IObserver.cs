
namespace QuoteMonitor;

public abstract class IObserver
{
    protected abstract void SendMessage(AlertMessage alert);
    protected int? number;
    protected static int s_id = 1;
    protected IObserver()
    {
        number = IObserver.s_id;
        IObserver.s_id++;
    }

    public void Update(AlertMessage alert)
    {
        //PrintMsg(alert);
        SendMessage(alert);
    }

    void PrintMsg(AlertMessage alert)
    {
        Console.WriteLine("Observador numero: " + number + " recebendo mensagem --- " + alert!.priceState);
    }
};



