using System.Net.Mail;
namespace QuoteMonitor;

public static class MailBox
{
    static MailMessage message = new MailMessage
    {
        From = new MailAddress("mateus.l.silveira@gmail.com"),
        Subject = "QUOTE MONITOR ALERT",
        IsBodyHtml = true
    };
    public static MailMessage GetMailMessage(AlertMessage alert)
    {
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
                margin-bottom: 20px;""> Price: {alert.info.marketPrice} BRL</p>
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

}