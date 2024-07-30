using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Delete
{
    public class DeleteMaintCorrectiveEvent : BaseEvent
    {
        public MaintCorrective _maintCorrective { get; }

        public DeleteMaintCorrectiveEvent(MaintCorrective maintCorrective)
        {
            _maintCorrective = maintCorrective;
        }
    }
}