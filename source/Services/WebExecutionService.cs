using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json.Nodes;
using WebWacker.Models;

namespace WebWacker.Services
{
    public interface IWebExecutionService
    {
        Task<string?> Authenticate(Project project);

        Task<WebExecutionResult> Execute(Project project, WebExecution webExecution);
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

        public async Task<string?> Authenticate(Project project)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            var authDetails = project.AuthenticationEndpoint;
            if (authDetails == null)
            {
                throw new InvalidOperationException("Authentication endpoint cannot be null when Authenticating");
            }

            string url = String.Concat(project.BaseUrl, authDetails.Path);
            string body = authDetails.Body;

            if (project.Variables.Count > 0)
            {
                foreach (string key in project.Variables.Keys)
                {
                    url = url.Replace($"{{{{{key}}}}}", project.Variables[key]);
                    body = body.Replace($"{{{{{key}}}}}", project.Variables[key]);
                }

            }


            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            
            var responseBody = await response.Content.ReadAsStringAsync();

            JsonNode? jsonBody = JsonNode.Parse(responseBody);
            if (jsonBody == null)
            {
                return null;
            }

            string propertyName = authDetails.ResponseAccessTokenJsonPath.TrimStart('$', '.');
            string? accessToken = jsonBody[propertyName]?.ToString();

            return accessToken;
        }

        public async Task<WebExecutionResult> Execute(Project project, WebExecution webExecution)
        {
            HttpClient client = _httpClientFactory.CreateClient();

            if (!String.IsNullOrEmpty(webExecution.AccessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", webExecution.AccessToken);
            }

            string url = webExecution.Url;
            foreach (string key in project.Variables.Keys)
            {
                url = url.Replace($"{{{{{key}}}}}", project.Variables[key]);
            }

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
