using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Subjects.Commands.UpdateSubject
{
    public class SubjectUpdatedEvent : BaseEvent
    {
        public Subject Subject { get; set; }

        public SubjectUpdatedEvent(Subject subject)
        {
            Subject = subject;
        }
    }
}