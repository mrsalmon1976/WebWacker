using System.Text.Json.Serialization;

namespace WebWacker.Models
{
    public class Project
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("baseUrl")]
        public string BaseUrl { get; set; } = string.Empty;

        [JsonPropertyName("threadCount")]
        public int ThreadCount { get; set; } = 10;

        [JsonPropertyName("sequentialEndpoints")]
        public bool SequentialEndpoints { get; set; } = true;

        [JsonPropertyName("authenticationEndpoint")]
        public AuthenticationEndpoint? AuthenticationEndpoint { get; set; }

        [JsonPropertyName("endpoints")]
        public IEnumerable<ProjectEndpoint> Endpoints { get; set; } = Enumerable.Empty<ProjectEndpoint>();

        [JsonPropertyName("variables")]
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }
            if (ThreadCount <= 0)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                return false;
            }

            return true;
        }
    }
}
