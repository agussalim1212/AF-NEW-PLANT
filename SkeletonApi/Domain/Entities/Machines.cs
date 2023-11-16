using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class Machine : BaseAuditableEntity
    {
       
        [Column("name")]
        public string? Name { get; set; }
        
        public ICollection<CategoryMachineHasMachine> CategoryMachineHasMachines { get; set; } = new List<CategoryMachineHasMachine>();
        public ICollection<SubjectHasMachine> SubjectHasMachines { get; set; } = new List<SubjectHasMachine>();

        public ICollection<MaintenacePreventive> maintenacePreventives { get; set; } = new List<MaintenacePreventive>();

        public ICollection<MaintCorrective> maintCorrectives { get; set; } = new List<MaintCorrective>();

    }
}
