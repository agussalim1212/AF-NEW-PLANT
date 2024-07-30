using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.CategoryHasMachines.Commands.CreateCategoryHasMachine
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