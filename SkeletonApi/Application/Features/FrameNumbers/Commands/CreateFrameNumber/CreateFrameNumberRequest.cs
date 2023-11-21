using MediatR;
using SkeletonApi.Application.Features.Machines;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.FrameNumb.Commands.CreateFrameNumber
{
     public sealed record CreateFrameNumberRequest : IRequest<Result<CreateFrameNumberResponseDto>>
     {
        public string Vid { get; set; }
        public string Name { get; set; }

     }
}
