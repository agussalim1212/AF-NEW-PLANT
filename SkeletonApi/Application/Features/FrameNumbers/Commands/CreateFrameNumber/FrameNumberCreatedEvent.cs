using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.FrameNumb.Commands.CreateFrameNumber
{
    public class FrameNumberCreatedEvent : BaseEvent
    {
        public FrameNumber FrameNumber { get; set; }
        public FrameNumberCreatedEvent(FrameNumber frameNumber)
        {
            FrameNumber = frameNumber;
        }

    }
}
