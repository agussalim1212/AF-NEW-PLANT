using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.FrameNumberSubject.Commands.UpdateFrameNumberHasSubject
{
    public class FrameNumberSubjectUpdateEvent : BaseEvent
    {
        public FrameNumberHasSubjects FrameNumberHasSubjects { get; set; }
        public FrameNumberSubjectUpdateEvent(FrameNumberHasSubjects frameNumberHasSubjects)
        {
            FrameNumberHasSubjects = frameNumberHasSubjects;
        }
    }
}
