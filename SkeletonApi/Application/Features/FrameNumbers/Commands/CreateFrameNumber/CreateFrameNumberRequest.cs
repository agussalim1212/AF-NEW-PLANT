using MediatR;
using SkeletonApi.Application.Features.FrameNumbers;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.FrameNumbers.Commands.CreateFrameNumber
{
    public sealed record CreateFrameNumberRequest : IRequest<Result<CreateFrameNumberResponseDto>>
    {
        public string Vid { get; set; }
        public string Name { get; set; }
    }
}