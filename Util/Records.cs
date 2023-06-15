namespace QuoteMonitor;

public record AssetInfo(string assetName, double marketPrice, long referenceTimestamp, long requestTimestamp);
public record SmtpConfig(string host, int port, string username, string password);
