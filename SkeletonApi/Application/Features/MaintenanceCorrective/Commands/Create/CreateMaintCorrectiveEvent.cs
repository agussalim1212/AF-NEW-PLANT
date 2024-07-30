using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Create
{
    public class CreateMaintCorrectiveEvent : BaseEvent
    {
        public MaintCorrective _maintCorrective { get; set; }

        public CreateMaintCorrectiveEvent(MaintCorrective maintCorrective)
        {
            _maintCorrective = maintCorrective;
        }
    }
}