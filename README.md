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

## Usage

### Basic Setup

Add the service to your `Program.cs` or `Startup.cs`:

```csharp
using DynaScaleHangfire;

// Add services
builder.Services.AddHangfireDynaScale();

// Configure Hangfire
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(connectionString);
});

// Add middleware
app.UseHangfireDynaScale();
app.UseHangfireDashboard();
```

### Configuration

Configure the dynamic scaling settings in `appsettings.json`:

```json
{
  "HangfireDynaScale": {
    "MinWorkerCount": 1,
    "MaxWorkerCount": 10,
    "ScaleUpThreshold": 100,
    "ScaleDownThreshold": 10,
    "CheckIntervalSeconds": 30
  }
}
```

### API Endpoints

The package provides the following REST endpoints:

- `GET /dynamic-scaling/queues` - Get current queue status
- `GET /dynamic-scaling/health` - Get service health status
- `POST /dynamic-scaling/scale-up` - Manually trigger scale up
- `POST /dynamic-scaling/scale-down` - Manually trigger scale down

### JavaScript Integration

Include the dynamic scaling JavaScript in your Hangfire dashboard:

```html
<script src="/hangfire/js/dynamic-scaling.js"></script>
```

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