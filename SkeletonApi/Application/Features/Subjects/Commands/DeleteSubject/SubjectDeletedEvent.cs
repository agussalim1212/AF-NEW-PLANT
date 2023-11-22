using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Application.Features.Subjects.Commands.DeleteSubject
{
    public class SubjectDeletedEvent : BaseEvent
    {
        public Subject Subject { get; }

        public SubjectDeletedEvent(Subject subject)
        {
            Subject = subject;
        }
    }
}