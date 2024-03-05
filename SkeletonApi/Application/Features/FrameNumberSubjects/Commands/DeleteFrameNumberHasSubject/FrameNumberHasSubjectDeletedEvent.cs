using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.FrameNumberSubject.Commands.DeleteFrameNumberHasSubject
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
