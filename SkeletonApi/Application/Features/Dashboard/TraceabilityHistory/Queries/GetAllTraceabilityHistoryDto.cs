using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Dashboard.TraceabilityHistory.Queries
{
    public class GetAllTraceabilityHistoryDto : IMapFrom<GetAllTraceabilityHistoryDto>
    {
        [JsonPropertyName("frame_number")]
        public string EngineId { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("result")]
        public string Status { get; set; }
    }
}