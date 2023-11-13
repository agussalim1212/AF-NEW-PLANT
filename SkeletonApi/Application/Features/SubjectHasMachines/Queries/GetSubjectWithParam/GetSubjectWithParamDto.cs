using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Queries.GetSubjectWithParam
{
    public class GetSubjectWithParamDto : IMapFrom<GetSubjectWithParamDto>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("subject")]
        public string? Subject { get; set; }
    }
}
