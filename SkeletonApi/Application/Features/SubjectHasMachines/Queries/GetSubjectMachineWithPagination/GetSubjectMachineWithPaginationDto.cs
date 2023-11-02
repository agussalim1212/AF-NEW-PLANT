using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Queries.GetSubjectMachineWithPagination
{
    public class GetSubjectMachineWithPaginationDto : IMapFrom<SubjectHasMachine>
    {
        [JsonPropertyName("machine_id")]
        public Guid MachineId { get; set; }
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime? UpdatedAt { get; set; } 
        [JsonPropertyName("subjects")]
        public List<SubjectDto> Subjects {get; set;}
    }

    public class SubjectDto : IMapFrom<Subject>
    {
        [JsonPropertyName("subject_id")]
        public Guid SubjectId { get; init; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; init; }
    }
}
