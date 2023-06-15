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
        string css = @"
        <style>
            body {
                background-color: #f5f5f5;
                font-family: Arial, sans-serif;
                text-align: center;
            }
            h1 {
                color: #333;
                font-size: 24px;
                margin-top: 40px;
            }
            p {
                color: #555;
                font-size: 18px;
                margin-bottom: 20px;
            }
        </style>
    ";

        string html = $@"
        <html>
        <head>
            {css}
        </head>
        <body>
            <h1>QUOTE MONITOR REPORT</h1>
            <p>START {action} {alert.info.assetName}</p>
            <p>Reference Time: {referenceTime}</p>
            <p>Request Time: {requestTime}</p>
        </body>
        </html>";
        message.Body = html;

        return message;
    }

}