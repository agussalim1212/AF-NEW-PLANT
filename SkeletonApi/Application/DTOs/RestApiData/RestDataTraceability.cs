using System.Text.Json.Serialization;

namespace SkeletonApi.Application.DTOs.RestApiData
{
    public class RestDataTraceability
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("v")]
        public virtual object Value { get; set; }

        [JsonPropertyName("q")]
        public bool Quality { get; set; }

        [JsonPropertyName("t")]
        public long Time { get; set; }
    }
}