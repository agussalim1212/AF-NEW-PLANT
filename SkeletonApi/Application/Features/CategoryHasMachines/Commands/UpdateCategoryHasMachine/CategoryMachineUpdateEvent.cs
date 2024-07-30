using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.CategoryHasMachines.Commands.UpdateCategoryHasMachine
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