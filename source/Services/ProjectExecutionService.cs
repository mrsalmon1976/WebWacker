using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using WebWacker.Logging;
using WebWacker.Models;

namespace WebWacker.Services
{
    public interface IProjectExecutionService
    {
        Task<List<WebExecutionResult>> ExecuteProject(Project project, CancellationToken stoppingToken);
    }
    public class ProjectExecutionService : IProjectExecutionService
    {
        private readonly ILogger<ProjectExecutionService> _logger;
        private readonly IWebExecutionService _webExecutionService;

        private ConcurrentBag<WebExecutionResult> _webExecutions = new ConcurrentBag<WebExecutionResult>();

        public ProjectExecutionService(ILogger<ProjectExecutionService> logger, IWebExecutionService webExecutionService)
        {
            _logger = logger;
            _webExecutionService = webExecutionService;
        }

        public async Task<List<WebExecutionResult>> ExecuteProject(Project project, CancellationToken stoppingToken)
        {
            _logger.LogInformation(LogColor.Blue, $"Executing project '{project.Name}' with {project.ThreadCount} threads.");

            var tasks = CreateExecutionTasks(project, stoppingToken);

            using var semaphore = new SemaphoreSlim(project.ThreadCount, project.ThreadCount);

            var runningTasks = tasks.Select(async task =>
            {
                await semaphore.WaitAsync();
                try
                {
                    if (!stoppingToken.IsCancellationRequested)
                    {
                        await task();
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            await Task.WhenAll(runningTasks);

            if (stoppingToken.IsCancellationRequested)
            {
                _webExecutions.Clear();
            }

            return _webExecutions.ToList();
        }

        private List<Func<Task>> CreateExecutionTasks(Project project, CancellationToken stoppingToken)
        {
            var tasks = new List<Func<Task>>();
            Random r = new Random();

            List<WebExecution> webExecutions = new List<WebExecution>();
            
            // create the web execution models, and order them if sequential endpoints are enabled
            foreach (ProjectEndpoint endpoint in project.Endpoints)
            {
                string url = String.Concat(project.BaseUrl, endpoint.Path);
                for (int i = 0; i < endpoint.HitCount; i++)
                {
                    webExecutions.Add(new WebExecution
                    {
                        Url = url,
                        Order = (project.SequentialEndpoints ? 0 : r.Next(0, Int32.MaxValue))
                    });

                    if (stoppingToken.IsCancellationRequested)
                    {
                        tasks.Clear();
                        break;
                    }

                }
            }

            if (!project.SequentialEndpoints)
            {
                webExecutions = webExecutions.OrderBy(x => x.Order).ToList();
            }

            // create execution as tasks for each execution
            foreach (WebExecution webExecution in webExecutions)
            {
                tasks.Add(async () =>
                {
                    var result = await _webExecutionService.Execute(webExecution);
                    _webExecutions.Add(result);
                });

                if (stoppingToken.IsCancellationRequested)
                {
                    tasks.Clear();
                    break;
                }
            }
            _logger.LogInformation($"Created {tasks.Count} web execution tasks for project '{project.Name}'.");
            return tasks;
        }

    }
}
