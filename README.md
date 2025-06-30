# DynaScaleHangfire

A dynamic scaling extension for Hangfire that provides real-time queue monitoring and automatic server scaling capabilities.

## Features

- **Real-time Queue Monitoring**: Monitor job queue lengths and processing status
- **Dynamic Server Scaling**: Automatically scale Hangfire servers based on queue load
- **RESTful API**: Simple HTTP endpoints for queue management
- **Web Dashboard Integration**: Seamless integration with Hangfire dashboard
- **Configurable Scaling Rules**: Customizable thresholds and scaling policies
- **Health Monitoring**: Built-in health checks and status reporting

## Installation

### NuGet Package

```bash
dotnet add package DynaScaleHangfire
```

### Manual Installation

1. Clone this repository
2. Build the project
3. Reference the built assembly in your project

## Quick Start

### 1. Add Services

```csharp
using Hangfire.DynaScale.Extensions;
using Hangfire.DynaScale.Models;

var builder = WebApplication.CreateBuilder(args);

// Add DynaScale services
builder.Services.AddHangfireDynaScale(new HangfireSettings
{
    MinWorkerCount = 1,
    MaxWorkerCount = 10,
    ScaleUpThreshold = 5,
    ScaleDownThreshold = 2,
    CheckIntervalSeconds = 30
});
```

### 2. Configure Middleware

**Option A: Automatic Setup (Recommended)**
```csharp
var app = builder.Build();

// Automatically creates wwwroot directory and adds static files middleware
app.UseHangfireDynaScaleWithStaticFiles();
```

**Option B: Manual Setup**
```csharp
var app = builder.Build();

// Add static files middleware manually
app.UseStaticFiles();

// Add DynaScale routes
app.UseHangfireDynaScale();
```

### 3. Access Dashboard

Navigate to `/dynamic-scaling` to access the DynaScale dashboard.

## Configuration

The `HangfireSettings` class allows you to configure:

- `MinWorkerCount`: Minimum number of worker processes
- `MaxWorkerCount`: Maximum number of worker processes  
- `ScaleUpThreshold`: Queue length threshold to trigger scale up
- `ScaleDownThreshold`: Queue length threshold to trigger scale down
- `CheckIntervalSeconds`: How often to check queue status

## How It Works

DynaScaleHangfire monitors your Hangfire job queues in real-time and automatically scales the number of worker processes based on the configured thresholds. When the queue length exceeds the scale-up threshold, it increases the worker count. When the queue length falls below the scale-down threshold, it decreases the worker count.

The web dashboard provides real-time monitoring of:
- Current queue lengths
- Active worker count
- Scaling history
- System performance metrics

## Automatic wwwroot Creation

This package automatically creates a `wwwroot` directory in your project if it doesn't exist when using `UseHangfireDynaScaleWithStaticFiles()`. This ensures that Hangfire's dashboard static files are properly served.

## Project Structure

```
DynaScaleHangfire/
├── Controllers/
│   └── DynaScaleController.cs          # REST API endpoints
├── Extensions/
│   ├── ApplicationBuilderExtensions.cs # Middleware configuration
│   └── ServiceCollectionExtensions.cs  # DI configuration
├── Models/
│   └── HangfireSettings.cs            # Configuration models
├── Services/
│   ├── HangfireServerManager.cs       # Core scaling logic
│   └── IHangfireServerManager.cs      # Service interface
└── wwwroot/
    └── js/
        └── dynamic-scaling.js         # Frontend JavaScript
```

## Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| MinWorkerCount | int | 1 | Minimum number of worker servers |
| MaxWorkerCount | int | 10 | Maximum number of worker servers |
| ScaleUpThreshold | int | 100 | Queue length threshold for scaling up |
| ScaleDownThreshold | int | 10 | Queue length threshold for scaling down |
| CheckIntervalSeconds | int | 30 | Interval between scaling checks |

## Development

### Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or VS Code
- SQL Server (for Hangfire storage)

### Building

```bash
dotnet restore
dotnet build
dotnet test
```

### Testing

```bash
dotnet test
```

### Creating NuGet Package

```bash
dotnet pack -c Release
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

- Project Link: [https://github.com/yourusername/DynaScaleHangfire](https://github.com/yourusername/DynaScaleHangfire)
- Issues: [https://github.com/yourusername/DynaScaleHangfire/issues](https://github.com/yourusername/DynaScaleHangfire/issues)

## Acknowledgments

- Built on top of [Hangfire](https://www.hangfire.io/)
- Inspired by modern microservices scaling patterns
- Community contributions and feedback 