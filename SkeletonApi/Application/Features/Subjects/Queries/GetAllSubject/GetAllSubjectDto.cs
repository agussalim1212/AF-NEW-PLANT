using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject
{
    public class GetAllSubjectDto : IMapFrom<Subject>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Vid { get; init; }

        [JsonIgnore]
      

        public Guid MasterSubjectId { get; init; }
        public string MasterSubjectName { get; init; }
    }
}