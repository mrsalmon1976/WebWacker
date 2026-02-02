using System.Text.Json.Serialization;

namespace WebWacker.Models
{
    public class AuthenticationEndpoint
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        [JsonPropertyName("responseAccessTokenJsonPath")]
        public string ResponseAccessTokenJsonPath { get; set; } = string.Empty;


    }
}
