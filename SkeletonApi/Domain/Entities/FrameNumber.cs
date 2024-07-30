using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities
{
    public class FrameNumber : BaseAuditableEntity
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("vid")]
        public string Vid { get; set; }

        public ICollection<FrameNumberHasSubjects> FrameNumberHasSubjects { get; set; } = new List<FrameNumberHasSubjects>();
    }
}