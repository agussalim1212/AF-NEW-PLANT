using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities
{
    public class SubjectHasMachine : BaseManyToMany
    {
        [Column("machine_id")]
        public Guid MachineId { get; set; }

        [Column("subject_id")]
        public Guid SubjectId { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public Machine Machine { get; set; }
        public Subject Subject { get; set; }
    }
}