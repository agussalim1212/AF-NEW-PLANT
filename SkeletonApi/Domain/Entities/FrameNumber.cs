using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class FrameNumber : BaseAuditableEntity
    {
        [Column("name")]
        public string Name { get; set; }
        [Column("vid")]
        public string Vid { get; set; }
        public ICollection<FrameNumberHasSubject> FrameNumberHasSubjects { get; set; } = new List<FrameNumberHasSubject>();
    }
}
