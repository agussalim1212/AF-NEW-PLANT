using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetSubjectById
{
    public class GetSubjectByIdDto : IMapFrom<Subject>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Vid { get; init; }
        public Guid MasterSubjectId { get; init; }
    }
}