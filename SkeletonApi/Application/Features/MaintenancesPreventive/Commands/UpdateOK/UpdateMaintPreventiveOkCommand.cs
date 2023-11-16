using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Create;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Update;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UpdateOK
{
    public record UpdateMaintPreventiveOkCommand : IRequest<Result<UpdateMaintPreventiveOkDto>>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("end_date")]
        public DateOnly EndDate { get; set; }

        [NotMapped] public bool ok { get; set; }


    }
    internal class UpdateMaintPreventiveOkCommandHandler : IRequestHandler<UpdateMaintPreventiveOkCommand, Result<UpdateMaintPreventiveOkDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMaintPreventiveOkCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UpdateMaintPreventiveOkDto>> Handle(UpdateMaintPreventiveOkCommand request, CancellationToken cancellationToken)
        {

            var MaintPrevOk = await _unitOfWork.Repository<MaintenacePreventive>().GetByIdAsync(request.Id);
            if (MaintPrevOk != null)
            {
                MaintPrevOk.EndDate = request.EndDate;
                MaintPrevOk.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<MaintenacePreventive>().UpdateAsync(MaintPrevOk, request.Id);
                MaintPrevOk.AddDomainEvent(new MaintPreventiveUpdateOkEvent(MaintPrevOk));
                var maintPrevDtoOk = _mapper.Map<UpdateMaintPreventiveOkDto>(MaintPrevOk);
                var machine = _unitOfWork.Repository<Machine>().Entities
                    .Where(x => x.Id == maintPrevDtoOk.MachineId)
                    .FirstOrDefault();
                var data = new UpdateMaintPreventiveOkDto()
                {
                    Id = maintPrevDtoOk.Id,
                    MachineId = maintPrevDtoOk.MachineId,
                    Name = machine.Name,
                    Plan = maintPrevDtoOk.Plan,
                    StartDate = (DateOnly)maintPrevDtoOk.StartDate,
                    Actual = maintPrevDtoOk.Actual,
                    EndDate = maintPrevDtoOk.EndDate,
                    ok = maintPrevDtoOk.EndDate == null ? false : true,
                };

                await _unitOfWork.Save(cancellationToken);
                return await Result<UpdateMaintPreventiveOkDto>.SuccessAsync(data, "Maintenance Preventive OK Updated");

            }
            return await Result<UpdateMaintPreventiveOkDto>.FailureAsync("Maintenance Preventive OK Not Found");
        }

    }
}
