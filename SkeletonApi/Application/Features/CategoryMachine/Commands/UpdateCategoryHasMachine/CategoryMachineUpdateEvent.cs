using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Commands.UpdateCategoryHasMachine
{
    public class CategoryMachineUpdateEvent : BaseEvent
    {
        public CategoryMachineHasMachine CategoryMachines { get; set; }
        public CategoryMachineUpdateEvent(CategoryMachineHasMachine categoryMachines) 
        {
            CategoryMachines = categoryMachines;
        
        }
    }
}
