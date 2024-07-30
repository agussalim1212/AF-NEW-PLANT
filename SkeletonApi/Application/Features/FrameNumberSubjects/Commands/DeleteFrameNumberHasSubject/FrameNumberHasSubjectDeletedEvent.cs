using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.FrameNumberSubjects.Commands.DeleteFrameNumberHasSubject
{
    public class FrameNumberHasSubjectDeletedEvent : BaseEvent
    {
        public FrameNumberHasSubjects FrameNumberHasSubjects { get; set; }

        public FrameNumberHasSubjectDeletedEvent(FrameNumberHasSubjects frameNumberHasSubjects)
        {
            FrameNumberHasSubjects = frameNumberHasSubjects;
        }
    }
}