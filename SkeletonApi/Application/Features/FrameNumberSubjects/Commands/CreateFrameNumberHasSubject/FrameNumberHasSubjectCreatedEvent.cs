using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.FrameNumberSubject.Commands.CreateFrameNumberHasSubject
{
    public class FrameNumberHasSubjectCreatedEvent : BaseEvent
    {
       public FrameNumberHasSubjects FrameNumberHasSubjects { get; set; }
        public FrameNumberHasSubjectCreatedEvent(FrameNumberHasSubjects frameNumberHasSubjects)
        {
            FrameNumberHasSubjects = frameNumberHasSubjects;
        }
    }
}
