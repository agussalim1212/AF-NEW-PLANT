using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Commands.DeleteSubjectHasMachine
{
    public class SubjectHasMachineDeleteEvent : BaseEvent
    {
        public SubjectHasMachine SubjectHasMachine { get; set; }

        public SubjectHasMachineDeleteEvent(SubjectHasMachine subjectHasMachine)
        {
            SubjectHasMachine = subjectHasMachine;
        }
    }
}