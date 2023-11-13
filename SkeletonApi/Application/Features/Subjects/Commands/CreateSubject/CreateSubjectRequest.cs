using MediatR;
using SkeletonApi.Application.Features.Machines;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Subjects.Commands.CreateSubject
{
    public sealed record CreateSubjectRequest : IRequest<Result<CreateSubjectResponseDto>>
    {
        public string Vid { get; set; }

        public string Subjects { get; set; }

    }
}
