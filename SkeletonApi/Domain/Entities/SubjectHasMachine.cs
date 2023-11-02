using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class SubjectHasMachine : BaseAuditableEntity
    {
        [Column("machine_id")]
        public Guid MachineId { get; set; }
        [Column("subject_id")]
        public Guid SubjectId { get; set; }

        public Machine Machine { get; set; }
        public Subject Subject { get; set; }
    }
}
