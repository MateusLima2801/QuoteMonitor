# Input via command line

The input is at the form: **asset, maxLimit, minLimit** where the limits are doubles in BRL.

Example:
`dotnet run PETR4 30.3 30.0`

# SMTP Configurations

The configurations in **appsettings.json** must be replaced for adjusting to the server chosen and for extracting the target emails, to send messages through.

An app's password is recommended to be create for using as credentials to the SMTP server.

# SMS Vonage API Configuration

A Vonage Account (`api-key`, `api-secret`) is necessary for using the SMS module and its attributes must be added at the **appsettings.json** file. A pair of it was provided, make sure of replacing the `target-phone-number` though.

If the free demo version is over, one can simply comment the **SetSMSConfig** function in Program.cs.

Apart of that, phone numbers must be added with only numeric characteres.

# External Sources

1. https://brapi.dev/: REST API for data collection
2. https://developer.vonage.com/en/home: REST API for SMS communication
3. https://google.com: SMTP credentials for e-mail communication
