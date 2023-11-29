using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLine
{
    public class GetListQualityMainLineDto : IMapFrom<GetListQualityMainLineDto>
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("frq_inverter")]
        public decimal FrqInverter { get; set; }
        [JsonPropertyName("duration_stop")]
        public decimal DurationStop { get; set; }
    }
}
