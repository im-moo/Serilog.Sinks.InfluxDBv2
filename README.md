# Serilog Sinks InfluxDB V2

A serilog sink that writes events to InfluxDB.

## Getting started

To use the InfluxDB sink, first install the [Nugget package](https://nuget.org/packages/serilog.sinks.console):

```bash
Install-Package Serilog.Sinks.InfluxDBv2
```

Then enable the sink using `WriteTo.InfluxDBv2()`:

```csharp
Log.Logger = new LoggerConfiguration()
  .WriteTo.InfluxDB("Api", "http://127.0.0.1:8086", "MyApplication", "MyAccessToken")
  .CreateLogger();
```

## Json `appsettings.json` configuration

To use the console sink with Microsoft.Extensions.Configuration, for example with ASP.NET Core or .NET Core, use the [Serilog.Settings.Configuration]("https://github.com/serilog/serilog-settings-configuration") package. First install that package if you have not already done so:

```csharp
Install-Package Serilog.Settings.Configuration
```

Instead of configuring the sink directly in code, call `ReadFrom.Configuration()`:

```bash
var configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json")
  .Build();

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(configuration)
  .CreateLogger();
```

In your `appsettings.json` file, under the `Serilog` node, :

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Console",
        "Args":[
          "source": "Api",
          "address": "http://127.0.0.1:8086",
          "bucket": "MyApplication",
          "token": "MyAccessToken"
        ]
      }
    ]
  }
}
```
