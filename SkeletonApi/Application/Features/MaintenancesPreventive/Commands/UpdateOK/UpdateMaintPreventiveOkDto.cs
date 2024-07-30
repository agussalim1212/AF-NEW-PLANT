using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UpdateOK
{
    public class UpdateMaintPreventiveOkDto : IMapFrom<MaintenacePreventive>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("machine_id")]
        public Guid? MachineId { get; set; }

        [JsonPropertyName("name")]
        [NotMapped] public string Name { get; set; }

        [JsonPropertyName("plan")]
        public string Plan { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("actual")]
        public string Actual { get; set; }

        [JsonPropertyName("end_date")]
        public DateOnly? EndDate { get; set; }

        [NotMapped] public bool ok { get; set; }
    }
}