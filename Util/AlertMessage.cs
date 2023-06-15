namespace QuoteMonitor;

public class AlertMessage
{
    public ePriceState priceState { get; private set; }
    public AssetInfo info { get; private set; }

    public AlertMessage(ePriceState PriceState, AssetInfo Info)
    {
        if (PriceState == ePriceState.insideLimits)
            throw new ArgumentException("invalid state for alert.");

        priceState = PriceState;
        info = Info;
    }
}