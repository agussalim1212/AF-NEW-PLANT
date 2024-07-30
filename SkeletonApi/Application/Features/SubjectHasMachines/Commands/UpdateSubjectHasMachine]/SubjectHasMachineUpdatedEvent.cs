using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Commands.UpdateSubjectHasMachine_
{
    public class SubjectHasMachineUpdatedEvent : BaseEvent
    {
        public SubjectHasMachine SubjectHasMachine { get; set; }

        public SubjectHasMachineUpdatedEvent(SubjectHasMachine subjectHasMachine)
        {
            SubjectHasMachine = subjectHasMachine;
        }
    }
}