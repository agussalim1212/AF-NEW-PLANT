using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Update
{
    public class UpdateMaintCorrectiveEvent : BaseEvent
    {
        public MaintCorrective _maintCorrective { get; set; }

        public UpdateMaintCorrectiveEvent(MaintCorrective maintCorrective)
        {
            _maintCorrective = maintCorrective;
        }
    }
}