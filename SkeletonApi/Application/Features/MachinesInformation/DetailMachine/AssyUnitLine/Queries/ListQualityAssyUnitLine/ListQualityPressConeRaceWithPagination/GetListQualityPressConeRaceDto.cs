using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityPressConeRaceWithPagination
{
    public class GetListQualityPressConeRaceDto
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("kedalaman")]
        public decimal Kedalaman { get; set; }

        [JsonPropertyName("tonase")]
        public decimal Tonase { get; set; }
    }
}