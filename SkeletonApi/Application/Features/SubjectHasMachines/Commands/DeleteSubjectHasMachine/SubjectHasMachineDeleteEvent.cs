using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
