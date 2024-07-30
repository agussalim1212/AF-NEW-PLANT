using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Update
{
    public class UpdateMaintCorrectiveDto : IMapFrom<MaintCorrective>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("machine_id")]
        public Guid? MachineId { get; set; }

        [JsonPropertyName("name")]
        [NotMapped] public string Name { get; set; }

        [JsonPropertyName("actual")]
        public string Actual { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateOnly? EndDate { get; set; }
    }
}