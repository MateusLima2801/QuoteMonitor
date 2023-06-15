using System.Collections.Concurrent;
namespace QuoteMonitor;

public abstract class IObjectPool<Object, Config> : IDisposable
{
    private BlockingCollection<Object> pool;
    private SemaphoreSlim semaphore;

    public IObjectPool(Config config, int poolSize)
    {
        pool = new BlockingCollection<Object>();
        semaphore = new SemaphoreSlim(poolSize);

        // Create and populate the pool with SmtpClient instances
        for (int i = 0; i < poolSize; i++)
        {
            pool.Add(Initializer(config));
        }
    }

    public abstract Object Initializer(Config config);


    public async Task<Object> GetObject()
    {
        await semaphore.WaitAsync();
        return pool.Take();
    }

    public void ReturnObject(Object Object)
    {
        pool.Add(Object);
        semaphore.Release();
    }

    public void Dispose()
    {
        pool.CompleteAdding();

        foreach (var obj in pool)
        {
            if (obj is IDisposable disposableObj)
            {
                disposableObj.Dispose();
            }
        }

        pool.Dispose();
    }
}