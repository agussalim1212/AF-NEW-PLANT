using SkeletonApi.Application.Common.Mappings;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Queries.GetAll
{
    public class GetAllMaintCorrectiveDto : IMapFrom<GetAllMaintCorrectiveDto>
    {
        [JsonPropertyName("count_actual")]
        [NotMapped] public decimal? CountActual { get; set; }

        [JsonPropertyName("label")]
        [NotMapped]
        public string? label { get; set; }

        [JsonPropertyName("date_time")]
        public DateOnly StartDate { get; set; }
    }
}