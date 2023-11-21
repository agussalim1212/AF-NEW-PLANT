using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Application.Features.FrameNumb.Commands.UpdateFrameNumber
{
    public class FrameNumberUpdateEvent : BaseEvent
    {
        public FrameNumber FrameNumber { get; set; }
        public FrameNumberUpdateEvent(FrameNumber frameNumber) 
        {
            FrameNumber = frameNumber;
        }
    }
}
