using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
