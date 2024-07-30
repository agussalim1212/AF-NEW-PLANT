using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.FrameNumbers.Commands.DeleteFrameNumber
{
    public class FrameNumberDeleteEvent : BaseEvent
    {
        public FrameNumber FrameNumber { get; set; }

        public FrameNumberDeleteEvent(FrameNumber frameNumber)
        {
            FrameNumber = frameNumber;
        }
    }
}