namespace QuoteMonitor;

public class EventHandler
{
    private int requestDelayMilliSec;
    MonitorInput input;
    Subject subject;
    ePriceState state = ePriceState.insideLimits;
    CancellationToken token;

    public EventHandler(MonitorInput Input, Subject Subject, CancellationToken Token, int RequestDelayMilliSec = 0)
    {
        requestDelayMilliSec = RequestDelayMilliSec <= DataHttpClient.minDelayMilliSeconds
                               ? DataHttpClient.minDelayMilliSeconds
                               : RequestDelayMilliSec;
        input = Input;
        subject = Subject;
        token = Token;
    }

    public async Task Start()
    {
        while (!token.IsCancellationRequested)
        {
            //Console.WriteLine("1000");
            AssetInfo? info = DataHttpClient.GetAssetInfo(input.assetName).Result;
            handleAssetInfo(info);
            await Task.Delay(requestDelayMilliSec, token);
        }
    }

    private void handleAssetInfo(AssetInfo? info)
    {
        if (info == null) throw new Exception("Problem in Asset Info Request");
        else if (state == ePriceState.insideLimits)
        {
            if (info.marketPrice < input.minLimit)
            {
                state = ePriceState.minOverflow;
                subject.CreateMsg(new AlertMessage(state, info));
            }
            else if (info.marketPrice > input.maxLimit)
            {
                state = ePriceState.maxOverflow;
                subject.CreateMsg(new AlertMessage(state, info));
            }
        }
        else if (info.marketPrice <= input.maxLimit && info.marketPrice >= input.minLimit)
        {
            state = ePriceState.insideLimits;
        }
    }
}

