using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.FrameNumberSubjects.Commands.UpdateFrameNumberHasSubject
{
    public class FrameNumberSubjectUpdateEvent : BaseEvent
    {
        public FrameNumberHasSubjects FrameNumberHasSubjects { get; set; }

        public FrameNumberSubjectUpdateEvent(FrameNumberHasSubjects frameNumberHasSubjects)
        {
            FrameNumberHasSubjects = frameNumberHasSubjects;
        }
    }
}