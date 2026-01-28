using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using WebWacker.Logging;
using WebWacker.Models;

namespace WebWacker.Services
{
    public interface IProjectSummaryService
    {
        ProjectSummary GenerateSummary(Project project, IEnumerable<WebExecutionResult> results);
    }
    public class ProjectSummaryService : IProjectSummaryService
    {
        private readonly ILogger<ProjectSummaryService> _logger;

        public ProjectSummaryService(ILogger<ProjectSummaryService> logger)
        {
            _logger = logger;
        }

        public ProjectSummary GenerateSummary(Project project, IEnumerable<WebExecutionResult> results)
        {
            _logger.LogInformation(LogColor.Green, $"---------------------------------------------------------");
            _logger.LogInformation(LogColor.Blue, $"Summary Results for Project '{project.Name}'");
            _logger.LogInformation($"---------------------------------------------------------");
            _logger.LogInformation($"Thread count: {project.ThreadCount}");

            var summary = results
                .GroupBy(x => new { x.Url, x.StatusCode })
                .Select(g => new
                {
                    Url = g.Key.Url,
                    StatusCode = g.Key.StatusCode,
                    Count = g.Count(),
                    AverageResponseTime = g.Average(x => x.ResponseTime)
                });

            _logger.LogInformation($"---------------------------------------------------------");

            foreach (var item in summary)
            {
                _logger.LogInformation(LogColor.Blue, $"URL: {item.Url}");
                _logger.LogInformation($"Status Code: {item.StatusCode}");
                _logger.LogInformation($"Request Count: {item.Count}");
                _logger.LogInformation($"Average Response Time: {item.AverageResponseTime} ms");
                _logger.LogInformation($"---------------------------------------------------------");
            }

            return new ProjectSummary();

            
        }
    }
}
