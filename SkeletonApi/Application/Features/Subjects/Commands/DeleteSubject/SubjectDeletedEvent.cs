using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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