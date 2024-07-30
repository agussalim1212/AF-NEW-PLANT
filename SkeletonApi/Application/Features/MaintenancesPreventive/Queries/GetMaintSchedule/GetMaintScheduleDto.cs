using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetMaintSchedule
{
    public class GetMaintScheduleDto : IMapFrom<MaintenacePreventive>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("plan")]
        public string? Plan { get; set; }

        [JsonPropertyName("date_time")]
        public DateOnly? StartDate { get; set; }
    }
}