using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Commands.CreateCategoryHasMachine
{
    public class CategoryCreatedEvent : BaseEvent
    {
        public CategoryMachineHasMachine CategoryMachine { get; set; }
        public CategoryCreatedEvent(CategoryMachineHasMachine categoryMachine)
        {
           CategoryMachine = categoryMachine;
        }
    }
}
