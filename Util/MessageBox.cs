using System.Net.Mail;
using System.Globalization;
namespace QuoteMonitor;

public static class MessageBox
{

    public static MailMessage GetMailMessage(AlertMessage alert, MailAddress address)
    {
        MailMessage message = new MailMessage
        {
            From = address,
            Subject = "QUOTE MONITOR ALERT",
            IsBodyHtml = true
        };

        string action = alert.priceState == ePriceState.maxOverflow ? "SELLING" : "BUYING";
        string referenceTime = Util.FromTimestampToDate(alert.info.referenceTimestamp);
        string requestTime = Util.FromTimestampToDate(alert.info.requestTimestamp);

        string html = $@"
        <html>
        <body>
             <span style=""
                font-family: Arial, sans-serif;
                text-align: center;"">
            <h1 style=""color: #333;
                font-size: 24px;
                margin-top: 40px;"">QUOTE MONITOR REPORT</h1>
            <h2 style=""color: #333;
                margin-top: 20px; "">START {action} {alert.info.assetName}</h2>
            <p style=""color: #555;
                font-size: 18px;
                margin-bottom: 20px;""> Price: {alert.info.marketPrice.ToString(CultureInfo.InvariantCulture)} BRL</p>
            <p style=""color: #555;
                font-size: 16px;
                margin-bottom: 0.5em;
                font-weight: light"">Reference Time: {referenceTime}</p>
            <p styles=""color: #555;
                font-size: 16px;
                margin-bottom: 0.5em;
                font-weight: light"">Request Time: {requestTime}</p>
            </span>
        </body>
        </html>";
        message.Body = html;

        return message;
    }

    public static string GetSMSMessage(AlertMessage alert)
    {
        string action = alert.priceState == ePriceState.maxOverflow ? "SELLING" : "BUYING";
        string referenceTime = Util.FromTimestampToDate(alert.info.referenceTimestamp);
        string requestTime = Util.FromTimestampToDate(alert.info.requestTimestamp);

        string message = $"QUOTE MONITOR REPORT\n\n" +
                         $"START {action} {alert.info.assetName}\n\n" +
                         $"Price: {alert.info.marketPrice.ToString(CultureInfo.InvariantCulture)} BRL\n\n" +
                         $"Reference Time:\n" +
                         $"{referenceTime}\n\n" +
                         $"Request Time:\n" +
                         $"{requestTime}\n\n";


        return message;
    }

}