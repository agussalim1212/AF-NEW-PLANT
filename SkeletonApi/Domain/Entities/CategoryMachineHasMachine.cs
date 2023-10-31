using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class CategoryMachineHasMachine : BaseAuditableEntity
    {
      
        public Guid CategoryMachineId { get; set; }
        public Guid MachineId { get; set; }
    
        public Machine Machine { get; set; }
    
        public CategoryMachines CategoryMachine { get; set; }
    }
}
