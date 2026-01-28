using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using WebWacker.Logging;
using WebWacker.Models;
using WebWacker.Services;

public class WebWackerWorker : BackgroundService
{
    private readonly ILogger<WebWackerWorker> _logger;
    private readonly IProjectFileService _projectFileService;
    private readonly IProjectExecutionService _projectExecutionService;
    private readonly IProjectSummaryService _projectSummaryService;

    public WebWackerWorker(ILogger<WebWackerWorker> logger
        , IProjectFileService projectFileService
        , IProjectExecutionService projectExecutionService
        , IProjectSummaryService projectSummaryService
        )
    {
        _logger = logger;
        _projectFileService = projectFileService;
        _projectExecutionService = projectExecutionService;
        _projectSummaryService = projectSummaryService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IEnumerable<Project> projects = _projectFileService.LoadAllProjects();
        _logger.LogInformation($"Loaded {projects.Count()} projects.");

        foreach (var project in projects)
        {
            _logger.LogInformation($"Processing project: {project.Name}");

            ConcurrentBag<WebExecutionResult> webExecutionResults = new ConcurrentBag<WebExecutionResult>();
            var backgroundTasks = new List<Task>();

            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Cancellation requested, stopping project processing.");
                break;
            }

            var task = Task.Run(async () =>
            {
                var results = await _projectExecutionService.ExecuteProject(project, stoppingToken);
                foreach (var wer in results)
                {
                    webExecutionResults.Add(wer);
                }
            }, stoppingToken);

            backgroundTasks.Add(task);

            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Cancellation requested, stopping project processing.");
                break;
            }
            else
            {
                await Task.WhenAll(backgroundTasks);
                _projectSummaryService.GenerateSummary(project, webExecutionResults);

            }
        }

        _logger.LogInformation("All projects have been processed.");

        Console.ReadKey();
    }
}

