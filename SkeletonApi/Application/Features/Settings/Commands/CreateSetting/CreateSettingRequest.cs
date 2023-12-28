using MediatR;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Settings.Commands.CreateSetting
{
    public sealed record CreateSettingRequest : IRequest<Result<CreateSettingResponseDto>>
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
}
