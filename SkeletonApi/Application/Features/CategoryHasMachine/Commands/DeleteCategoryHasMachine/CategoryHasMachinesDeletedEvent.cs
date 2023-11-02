using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Commands.DeleteCategoryHasMachine
{
    public class CategoryHasMachinesDeletedEvent : BaseEvent
    {
        public CategoryMachineHasMachine CategoryMachinesHasMachines { get; set; }
        public CategoryHasMachinesDeletedEvent(CategoryMachineHasMachine categoryMachineHasMachine) 
        {
          CategoryMachinesHasMachines = categoryMachineHasMachine;
        }
    }
}
