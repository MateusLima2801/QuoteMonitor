namespace QuoteMonitor;

public class Subject
{
    List<IObserver> Observers = new List<IObserver>();
    AlertMessage? _alert;

    public void Attach(params IObserver[] obsArray)
    {
        foreach (var obs in obsArray)
        {
            Observers.Add(obs);
        }

    }

    public void Detach(params IObserver[] obsArray)
    {
        foreach (var obs in obsArray)
        {
            Observers.Remove(obs);
        }
    }

    private void Notify()
    {
        Parallel.ForEach(Observers, obs =>
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