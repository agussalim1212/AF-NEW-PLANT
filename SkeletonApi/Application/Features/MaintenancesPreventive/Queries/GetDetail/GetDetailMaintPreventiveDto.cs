using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetDetail
{
    public class GetDetailMaintPreventiveDto : IMapFrom<MaintenacePreventive>
    {
        [JsonPropertyName("name")]
        [NotMapped]public string? Name { get; init; }

        [JsonPropertyName("plan")]
        public string? Plan { get; init; }

        [JsonPropertyName("start_date")]
        public DateOnly? StartDate { get; init; }

        [JsonPropertyName("actual")]
        public string? Actual { get; init; }

        [JsonPropertyName("end_date")]
        public DateOnly? EndDate { get; init; }
    }
}
