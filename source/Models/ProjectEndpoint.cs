using System.Text.Json.Serialization;

namespace WebWacker.Models
{
    public class ProjectEndpoint
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;

        [JsonPropertyName("hitCount")]
        public int HitCount { get; set; } = 1;

        [JsonPropertyName("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        public bool IsValid()
        {
            return true;
        }
    }
}
