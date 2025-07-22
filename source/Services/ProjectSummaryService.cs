using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using WebWacker.Logging;
using WebWacker.Models;

namespace WebWacker.Services
{
    public interface IProjectSummaryService
    {
        ProjectSummary GenerateSummary(IEnumerable<WebExecutionResult> results);
    }
    public class ProjectSummaryService : IProjectSummaryService
    {
        private readonly ILogger<ProjectSummaryService> _logger;

        public ProjectSummaryService(ILogger<ProjectSummaryService> logger)
        {
            _logger = logger;
        }

        public ProjectSummary GenerateSummary(IEnumerable<WebExecutionResult> results)
        {
            _logger.LogInformation($"Generating summary...");

            var summary = results
                .GroupBy(x => new { x.Url, x.StatusCode })
                .Select(g => new
                {
                    Url = g.Key.Url,
                    StatusCode = g.Key.StatusCode,
                    Count = g.Count(),
                    AverageResponseTime = g.Average(x => x.ResponseTime)
                });

            _logger.LogInformation(LogColor.Blue, $"---------------------------------------------------------");

            foreach (var item in summary)
            {
                _logger.LogInformation($"URL: {item.Url}");
                _logger.LogInformation($"Status Code: {item.StatusCode}");
                _logger.LogInformation($"Count: {item.Count}");
                _logger.LogInformation($"Average Response Time: {item.AverageResponseTime} ms");
                _logger.LogInformation(LogColor.Blue, $"---------------------------------------------------------");
            }

            return new ProjectSummary();

            
        }
    }
}
