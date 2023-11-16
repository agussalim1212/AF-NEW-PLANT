using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
