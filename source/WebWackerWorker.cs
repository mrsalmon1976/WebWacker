using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

public class WebWackerWorker : BackgroundService
{
    private readonly ILogger<WebWackerWorker> _logger;

    public WebWackerWorker(ILogger<WebWackerWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Hello, World!");
            //_logger.LogInformation("WebWackerWorker is running at: {time}", DateTimeOffset.Now);

            //_logger.LogInformation("Colored message {Color}", ConsoleColor.Cyan);

            using (NLog.ScopeContext.PushProperty("Color", "Blue"))
            {
                _logger.LogInformation("Processing {Item}", "MyItem");
            }

            await Task.Delay(1000, stoppingToken); // Print every second
        }
    }
}

