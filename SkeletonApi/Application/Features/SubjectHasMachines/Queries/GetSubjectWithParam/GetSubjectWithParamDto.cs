using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

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