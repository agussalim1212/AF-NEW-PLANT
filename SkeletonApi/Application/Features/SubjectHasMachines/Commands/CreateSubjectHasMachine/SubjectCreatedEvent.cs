using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Commands.CreateSubjectHasMachine
{
    public class SubjectCreatedEvent : BaseEvent
    {
        public SubjectHasMachine SubjectHasMachine { get; set; }

        public SubjectCreatedEvent(SubjectHasMachine subjectHasMachine)
        {
            SubjectHasMachine = subjectHasMachine;
        }
    }
}