using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class Subject : BaseAuditableEntity
    {
        [Column("vid")]
        public string? Vid { get; set; }
        [Column("subject")]
        public string? Subjects { get; set; }

        public ICollection<SubjectHasMachine> SubjectHasMachines { get; set; } = new List<SubjectHasMachine>();
    }
}
