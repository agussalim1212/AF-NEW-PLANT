using SkeletonApi.Domain.Common.Abstracts;

namespace SkeletonApi.Domain.Entities
{
    public class CategoryMachineHasMachine : BaseManyToMany
    {
      
        public Guid CategoryMachineId { get; set; }
        public Guid MachineId { get; set; }
    
        public Machine Machine { get; set; }
    
        public CategoryMachines CategoryMachine { get; set; }
    }
}
