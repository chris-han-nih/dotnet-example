
## Add package
```bash
$ dotnet add package Serilog.AspNetCore
```

## Add configuration
```csharp
var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
builder.Logging.ClearProviders();
builder.Services.AddSerilog(logger);
```

## Serilog configuration
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Serilog": {
    "Using": [ "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "../logs/webapi-.log",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3}] {Username} {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

## Masking
### Add package
```bash
$ dotnet add package Serilog.Enrichers.Sensitive
$ dotnet add package Masking.Serilog --version 1.0.13
```

## Header Propagation
### Add package
```bash
$ dotnet add package Microsoft.AspNetCore.HeaderPropagation
```