using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Subjects.Commands.CreateSubject
{
    public class SubjectCreatedEvent : BaseEvent
    {
        public Subject Subject { get; set; }

        public SubjectCreatedEvent(Subject subject)
        {
            Subject = subject;
        }
    }
}