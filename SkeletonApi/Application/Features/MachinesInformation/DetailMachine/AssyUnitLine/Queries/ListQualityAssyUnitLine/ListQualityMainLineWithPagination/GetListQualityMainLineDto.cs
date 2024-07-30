using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLineWithPagination
{
    public class GetListQualityMainLineDto : IMapFrom<GetListQualityMainLineDto>
    {
        [JsonPropertyName("date_time")]
        public string? DateTime { get; set; }

        [JsonPropertyName("frq_inverter")]
        public decimal? FrqInverter { get; set; }
    }
}