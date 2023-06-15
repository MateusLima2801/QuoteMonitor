namespace QuoteMonitor;

public class MonitorInput
{
    public string assetName { get; private set; }
    public double minLimit { get; private set; }
    public double maxLimit { get; private set; }

    private MonitorInput(string _assetName, double _maxLimit, double _minLimit)
    {
        if (_maxLimit < _minLimit)
        {
            var t = _maxLimit;
            _maxLimit = _minLimit;
            _minLimit = t;
        }

        assetName = _assetName;
        minLimit = _minLimit;
        maxLimit = _maxLimit;
    }


    //correct entries: input = [string, double, double]
    public static MonitorInput handleInput(string[] input)
    {
        (string, double, double) tuple = ("", 0, 0);
        if (input.Length < 3) throw new Exception("Too few input arguments.");
        if (input.Length > 3) throw new Exception("Too many input arguments.");

        tuple.Item1 = input[0].ToUpper();
        if (checkAssetAvailability(tuple.Item1).Result == false) throw new Exception("Not available asset.");

        if (!double.TryParse(input[1], out tuple.Item2) || !double.TryParse(input[2], out tuple.Item3))
        {
            throw new ArgumentException("Invalid limits at input.");
        }

        return new MonitorInput(tuple.Item1, tuple.Item2, tuple.Item3);
    }

    private static async Task<bool> checkAssetAvailability(string asset)
    {
        var allAssets = await DataHttpClient.GetAllAssets();
        return (allAssets != null && allAssets.Contains(asset));
    }

}