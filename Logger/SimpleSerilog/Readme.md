# ASP.NET Core Simple Serilog

Web application to demonstrate implement rolling file logger with “Serilog” for .Net Core 1.1

## Setup

1. Download the source code and open the solution in Visual Studio 2017.
2. Build your application and run it. 
3. Open a browser and navigate to http://localhost:5000/api/tests
4. Go to the `Logs` folder and the test log should be generated.

You can configure the Serilog in the appsettings.json in the section of “Serilog”:

```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "RollingFile",
        "Args": {
          "pathFormat": "Logs\\log-{Date}.log",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{MachineName}][Thread:{ThreadId}] [{Level}] {SourceContext} - {Message}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

Serilog levels can be overridden per logging source as below, for example we only want to filter Microsoft and System category from Level Information and below.

```
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

If you would like to manually install Serilog packages for your own project, go to the Package Manager Console and run the command belows to install Serilog packages.

```
PM> Install-Package Serilog.Sinks.RollingFile -Version 3.3.0

PM> Install-Package Serilog.Extensions.Logging -Version 1.2.0

PM> Install-Package Serilog.Enrichers.Environment -Version 2.1.2

PM> Install-Package Serilog.Enrichers.Thread -Version 3.0.0

PM> Install-Package Serilog.Settings.Configuration -Version 2.4.0
```