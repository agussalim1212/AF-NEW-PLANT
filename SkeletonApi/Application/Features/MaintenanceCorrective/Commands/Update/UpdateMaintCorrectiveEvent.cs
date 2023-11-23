using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
