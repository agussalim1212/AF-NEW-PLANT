using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.FrameNumb.Commands.DeleteFrameNumber
{
    public record DeleteFrameNumberRequest : IRequest<Result<Guid>>, IMapFrom<FrameNumber>
    {
        public Guid Id { get; set; }

        public DeleteFrameNumberRequest(Guid id)
        {
            Id = id;
        }

        public DeleteFrameNumberRequest()
        {
        }
    }
}
