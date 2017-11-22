# ASP.NET Core Implementing Logging Provider

Web application to demonstrate implement Logging Provider for .Net Core
The reality is that we should have used an existing library like Serilog or others, but if you keen to build your own customized logger provider, try it out!

## Setup

1. Download the source code and open the solution in Visual Studio 2017.
2. Build your application and run it. 
3. Open a browser and navigate to http://localhost:5000/
4. Go to the `Logs` folder and the test log should be generated.

You can customized or adding more options in the appsettings.json:

```json
{
  "CustomLogger": {
    "Path": "Logs\\",
    "FilePrefix": "CustomLog_",
    "EmailSubject": "[Sample of Custom Logger Critical Email Message]",
    "ToEmail": "foo@bar.com",
    "CcEmails": "cc1@bar.com,cc2@bar.com"
  }
}
```

## Note

This is an example for demonstrate only and it is **Not Fully Tested**. For real project, you should rewrite the WriteToLogFile method in CustomLogger.cs 