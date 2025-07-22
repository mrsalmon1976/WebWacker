using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WebWacker.Models;

namespace WebWacker.Services
{
    public interface IWebExecutionService
    {
        Task<WebExecutionResult> Execute(WebExecution webExecution);
    }

    public class WebExecutionService : IWebExecutionService
    {
        private readonly ILogger<WebExecutionService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WebExecutionService(ILogger<WebExecutionService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WebExecutionResult> Execute(WebExecution webExecution)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            string url = webExecution.Url;
            Stopwatch timer = Stopwatch.StartNew();
            var response = await client.GetAsync(url);
            timer.Stop();

            var result = new WebExecutionResult
            {
                Url = url,
                StatusCode = (int)response.StatusCode,
                ResponseTime = timer.ElapsedMilliseconds
            };

            var logMessage = $"Executed {url} with status code {result.StatusCode} in {result.ResponseTime} ms";
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(logMessage);
            }
            else
            {
                _logger.LogError(logMessage);
            }

            return result;
        }
    }
}
