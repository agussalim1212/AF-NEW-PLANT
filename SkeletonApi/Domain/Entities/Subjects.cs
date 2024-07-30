using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities
{
    public class Subject : BaseAuditableEntity
    {
        [Column("vid")]
        public string? Vid { get; set; }

        [Column("subject")]
        public string? Subjects { get; set; }

        public ICollection<SubjectHasMachine> SubjectHasMachines { get; set; } = new List<SubjectHasMachine>();
        public ICollection<FrameNumberHasSubjects> FrameNumberHasSubjects { get; set; } = new List<FrameNumberHasSubjects>();
    }
}