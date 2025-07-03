# DynaScaleHangfire

A dynamic scaling extension for Hangfire that provides real-time server and queue monitoring with dynamic worker count management and actual server restart capabilities.

## Features

- **Real Server Restart**: Actually stop and restart Hangfire servers with new worker counts
- **Queue-based Worker Management**: Update worker counts for specific queues on individual servers
- **Job Processing Safety**: Automatically pause job processing before server restart
- **Active Server Monitoring**: Only display active servers (last heartbeat within 1 minute)
- **Queue Aggregation**: Combine duplicate queues within the same server
- **Modern UI**: Horizontal layout with improved queue management interface
- **Machine-based Grouping**: Group servers by machine name for better organization
- **RESTful API**: Simple HTTP endpoints for server-queue management
- **Web Dashboard Integration**: Seamless integration with Hangfire dashboard
- **Automatic Static Files**: JavaScript files are automatically copied to build output

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

// Add DynaScale services with custom options
builder.Services.AddHangfireDynaScale(new DynaScaleOptions
{
    MaxWorkerCountPerQueue = 50
});

// Or use default options
builder.Services.AddHangfireDynaScale();
```

### 2. Configure Middleware

```csharp
var app = builder.Build();

// Automatically creates wwwroot directory and adds static files middleware
app.UseHangfireDynaScaleWithStaticFiles();
```

### 3. Access Dashboard

Navigate to `/dynamic-scaling` to access the DynaScale dashboard.

## Configuration

### DynaScaleOptions

```csharp
public sealed record DynaScaleOptions
{
    public int MaxWorkerCountPerQueue { get; init; } = 100;
}
```

- `MaxWorkerCountPerQueue`: Maximum number of workers that can be set for any queue (default: 100)

## How It Works

DynaScaleHangfire provides real-time server management with actual server restart capabilities:

### Server Restart Process
1. **Pause Job Processing**: Temporarily pause new job assignments to the target server
2. **Wait for Completion**: Wait for currently processing jobs to complete
3. **Stop Server**: Remove the server from Hangfire's storage
4. **Restart Server**: Create a new BackgroundJobServer with updated worker count
5. **Resume Processing**: New jobs can be assigned to the restarted server

### Queue Management
- **Queue Aggregation**: Duplicate queues within the same server are combined
- **Worker Count Calculation**: Total worker count is calculated per queue per server
- **Active Server Filtering**: Only servers with recent heartbeats are displayed

### Dashboard Features
- **Horizontal Layout**: Queue information and controls are displayed side by side
- **Real-time Updates**: Automatic page refresh after worker count changes
- **Server Status**: Active/inactive status with last heartbeat information
- **Machine Grouping**: Servers are grouped by machine name for better organization

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
│   ├── DynaScaleOptions.cs            # Configuration options
│   ├── ServerInfoModel.cs             # Data models
│   └── SetWorkersRequest.cs           # API request model
├── Services/
│   ├── HangfireServerManager.cs       # Core server management logic
│   └── IHangfireServerManager.cs      # Service interface
├── Pages/
│   └── DynamicScalingPage.cs          # Dashboard page
├── build/
│   └── DynaScaleHangfire.targets      # MSBuild targets for file copying
└── wwwroot/
    └── js/
        └── dynamic-scaling.js         # Frontend JavaScript
```

## API Endpoints

- `GET /dynamic-scaling/servers` - Get all active server-queue configurations grouped by machine
- `POST /dynamic-scaling/servers/{serverName}/queues/{queueName}/set-workers` - Update worker count for a specific server-queue

### Request Body for Set Workers

```json
{
  "workerCount": 5,
  "applyToAllServers": false
}
```

- `workerCount`: The new worker count to set
- `applyToAllServers`: If true, applies the worker count to all servers for this queue

## Data Models

### ServerInfo

```csharp
public sealed record ServerInfo
{
    public string ServerName { get; init; }        // Machine name
    public bool IsActive { get; init; }            // Server activity status
    public DateTime LastHeartbeat { get; init; }   // Last heartbeat time
    public List<QueueInfo> Queues { get; init; }   // Queue configurations
}
```

### QueueInfo

```csharp
public sealed record QueueInfo
{
    public string ServerName { get; init; }        // Actual server name
    public string QueueName { get; init; }         // Queue name
    public int CurrentWorkerCount { get; init; }   // Current worker count
    public int MaxWorkerCount { get; init; }       // Maximum worker count
}
```

### SetWorkersRequest

```csharp
public sealed record SetWorkersRequest
{
    public int WorkerCount { get; init; }
    public bool ApplyToAllServers { get; init; }
}
```

## Key Features Explained

### Real Server Restart
Unlike other solutions that only update configuration, DynaScaleHangfire actually:
- Stops the existing BackgroundJobServer
- Creates a new BackgroundJobServer with updated settings
- Ensures job processing safety during the restart process

### Queue Aggregation
- Combines duplicate queue names within the same server
- Calculates total worker count for each unique queue
- Maintains individual server separation

### Active Server Filtering
- Only displays servers with heartbeats within the last minute
- Automatically hides inactive or disconnected servers
- Provides real-time server status monitoring

### Modern UI
- Horizontal layout for better space utilization
- Side-by-side queue information and controls
- Automatic page refresh after changes
- Responsive design for different screen sizes

## Development

### Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or VS Code
- Hangfire storage provider (SQL Server, Redis, PostgreSQL, etc.)

### Building

```bash
dotnet restore
dotnet build
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

## Contact

- Project Link: [https://github.com/anilterzi/DynaScaleHangfire](https://github.com/anilterzi/DynaScaleHangfire)
- Issues: [https://github.com/anilterzi/DynaScaleHangfire/issues](https://github.com/anilterzi/DynaScaleHangfire/issues)

## Acknowledgments

- Built on top of [Hangfire](https://www.hangfire.io/)
- Uses Hangfire's monitoring APIs for real-time data
- Community contributions and feedback 