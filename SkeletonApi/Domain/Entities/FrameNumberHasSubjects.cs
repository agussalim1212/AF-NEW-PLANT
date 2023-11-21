using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class FrameNumberHasSubjects : BaseManyToMany
    {
        [Column("frame_number_id")]
        public Guid FrameNumberId { get; set; }
        [Column("subject_id")]
        public Guid SubjectId { get; set; }

        public Subject Subject { get; set; }
        public FrameNumber FrameNumber { get; set; }
    }
}
