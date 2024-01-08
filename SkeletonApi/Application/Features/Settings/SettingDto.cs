using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.Settings
{
    public record SettingDto
    {
        [JsonPropertyName("machine")]
        public string Name { get; set; }
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
        [JsonPropertyName("minimum")]
        public decimal? Minimum { get; set; }
        [JsonPropertyName("medium")]
        public decimal? Medium { get; set; }
        [JsonPropertyName("maximum")]
        public decimal? Maximum { get; set; }
    }
    public sealed record CreateSettingResponseDto : SettingDto { }
}
