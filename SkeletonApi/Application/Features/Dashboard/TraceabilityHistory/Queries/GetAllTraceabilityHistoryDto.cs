using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities.Tsdb;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.Dashboard.Traceability_History.Queries
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
