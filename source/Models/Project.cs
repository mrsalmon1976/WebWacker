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

        [JsonPropertyName("endPoints")]
        public IEnumerable<ProjectEndpoint> Endpoints { get; set; } = Enumerable.Empty<ProjectEndpoint>();

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
