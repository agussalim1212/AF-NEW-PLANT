using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities
{
    public class Machine : BaseAuditableEntity
    {
        [Column("vid")]
        public string? Vid { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        public ICollection<CategoryMachineHasMachine> CategoryMachineHasMachines { get; set; } = new List<CategoryMachineHasMachine>();
        public ICollection<SubjectHasMachine> SubjectHasMachines { get; set; } = new List<SubjectHasMachine>();
    }
}