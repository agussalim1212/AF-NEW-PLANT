using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UploadExcel
{
    public class UploadExcelMaintPrevDto : IMapFrom<MaintenacePreventive>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        [NotMapped] public string? Name { get; set; }

        [JsonPropertyName("plan")]
        public string? Plan { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly? StartDate { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("actual")]
        public string? Actual { get; set; } = null;

        [JsonPropertyName("machine_id")]
        public Guid? MachineId { get; set; }

        [JsonPropertyName("end_date")]
        public DateOnly? EndDate { get; set; } = null;

        [NotMapped] public bool ok { get; set; } = false;
    }
}
