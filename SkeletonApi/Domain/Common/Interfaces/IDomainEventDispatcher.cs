using SkeletonApi.Domain.Common.Abstracts;

namespace SkeletonApi.Domain.Common.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(IEnumerable<BaseEntity> entitiesWithEvents);
    }
}