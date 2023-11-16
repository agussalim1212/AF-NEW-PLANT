using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
