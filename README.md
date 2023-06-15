# Instructions

## SMTP Configurations

The configurations in **appsettings.json** must be replaced for adjusting to the server chosen and for extracting the target emails, to send messages through.

An app's password is recommended to be create for using as credentials to the SMTP server.

## SMS Vonage API Configuration

An account **(api-key, api-secret)** is necessary for using the SMS module and its attributes must be added at the **appsettings.json** file. If the free demo version is over, one can simply comment the **SetSMSConfig** function in Program.cs.

Phone numbers must be added with only numeric characteres.

## Input via command line

The input is at the form: **asset, maxLimit, minLimit** where the limits are doubles in BRL.

Example:
`dotnet run PETR4 30.3 30.0`
