using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.CategoryHasMachines.Commands.DeleteCategoryHasMachine
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