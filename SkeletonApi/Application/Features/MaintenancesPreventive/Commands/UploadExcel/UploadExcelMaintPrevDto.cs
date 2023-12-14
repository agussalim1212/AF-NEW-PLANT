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
        [NotMapped] public string Name { get; set; }

        [JsonPropertyName("plan")]
        public string Plan { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("actual")]
        public string Actual { get; set; }

        [JsonPropertyName("machine_id")]
        public Guid? MachineId { get; set; }

        [JsonPropertyName("end_date")]
        public DateOnly? EndDate { get; set; }

        [NotMapped] public bool ok { get; set; }
    }
}
