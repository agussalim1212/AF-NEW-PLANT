using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject
{
    public class GetAllSubjectDto : IMapFrom<Subject>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string? Subjects { get; set; }
    }
}