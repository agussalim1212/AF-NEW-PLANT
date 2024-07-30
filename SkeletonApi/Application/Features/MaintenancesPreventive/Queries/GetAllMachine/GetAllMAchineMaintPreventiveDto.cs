using SkeletonApi.Application.Common.Mappings;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetAllMachine
{
    public class GetAllMAchineMaintPreventiveDto : IMapFrom<GetAllMAchineMaintPreventiveDto>
    {
        [JsonPropertyName("count_actual")]
        [NotMapped] public decimal? CountActual { get; set; }

        [JsonPropertyName("count_plan")]
        [NotMapped] public decimal? CountPlan { get; set; }

        [JsonPropertyName("label")]
        [NotMapped]
        public string? label { get; set; }

        [JsonPropertyName("date_time")]
        public DateOnly StartDate { get; set; }
    }
}