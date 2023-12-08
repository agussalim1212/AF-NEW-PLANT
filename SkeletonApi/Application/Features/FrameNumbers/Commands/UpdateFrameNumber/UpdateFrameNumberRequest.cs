using MediatR;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.FrameNumb.Commands.UpdateFrameNumber
{
    public record UpdateFrameNumberRequest : IRequest<Result<FrameNumber>>
    {
        public Guid Id { get; set; }
        public string Vid { get; set; }
        public string Name { get; set; }
    }
}
