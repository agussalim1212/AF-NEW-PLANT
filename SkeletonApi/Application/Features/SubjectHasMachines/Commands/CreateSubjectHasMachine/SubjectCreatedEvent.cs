using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
