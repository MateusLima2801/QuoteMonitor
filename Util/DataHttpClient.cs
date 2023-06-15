using System.Text.Json;

namespace QuoteMonitor;

public static class DataHttpClient
{
    private const string HOST = "https://brapi.dev/api";
    private const string ALL_ASSETS_ENDPOINT = "/available";
    private const string ASSET_INFO_ENDPOINT = "/quote";
    private static HttpClient httpClient = new();
    public static int minDelayMilliSeconds { get; private set; } = 1000;
    private static async Task<JsonElement> RequestGetHttp(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await httpClient.SendAsync(request);
        var jsonString = await response.Content.ReadAsStringAsync();
        JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement root = doc.RootElement;
        return root;
    }

    private static List<string> ParseAllAssets(JsonElement root)
    {
        JsonElement stocks = root.GetProperty("stocks");
        var allAssets = JsonSerializer.Deserialize<List<string>>(stocks);
        return allAssets!;
    }
    private static AssetInfo ParseAssetInfo(JsonElement root)
    {
        AssetInfo? info = null;
        string? assetName = root.GetProperty("results")[0].GetProperty("symbol").GetString();
        double? marketPrice = root.GetProperty("results")[0].GetProperty("regularMarketPrice").GetDouble();
        string? requestTime = root.GetProperty("requestedAt").GetString();
        long? requestTimestamp = Util.FromDateToTimestamp(requestTime!);
        string? referenceTime = root.GetProperty("results")[0].GetProperty("regularMarketTime").GetString();
        long? referenceTimestamp = Util.FromDateToTimestamp(referenceTime!);
        info = new AssetInfo(assetName!, (double)marketPrice, (long)referenceTimestamp, (long)requestTimestamp);
        return info;
    }

    public static async Task<List<string>?> GetAllAssets()
    {
        List<string>? allAssets = null;
        string url = $"{HOST}{ALL_ASSETS_ENDPOINT}";

        try
        {
            var root = await RequestGetHttp(url);
            allAssets = ParseAllAssets(root);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR AT {url}: {e}");
        }
        return allAssets;
    }

    public static async Task<AssetInfo?> GetAssetInfo(string asset)
    {
        AssetInfo? assetInfo = null;
        string url = $"{HOST}{ASSET_INFO_ENDPOINT}/{asset}";

        try
        {
            var root = await RequestGetHttp(url);
            assetInfo = ParseAssetInfo(root);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR AT {url}: {e}");
        }
        return assetInfo;
    }

}