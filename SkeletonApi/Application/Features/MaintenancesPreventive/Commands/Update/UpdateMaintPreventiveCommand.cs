using AutoMapper;
using MediatR;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UpdateOK;
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

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Update
{
    public record UpdateMaintPreventiveCommand : IRequest<Result<UpdateMaintPreventiveDto>>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        //[JsonPropertyName("name")]
        //public string? Name { get; init; }

        [JsonPropertyName("machine_id")]
        public Guid? MachineId { get; set; }

        [JsonPropertyName("plan")]
        public string? Plan { get; init; }

        [JsonPropertyName("actual")]
        public string? Actual { get; init; }

        [JsonPropertyName("start_date")]
        public DateOnly? StartDate { get; init; }

    }

    internal class UpdateMaintPreventiveCommandHandle : IRequestHandler<UpdateMaintPreventiveCommand, Result<UpdateMaintPreventiveDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMaintPreventiveCommandHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UpdateMaintPreventiveDto>> Handle(UpdateMaintPreventiveCommand request, CancellationToken cancellationToken)
        {
            var MaintPrev = await _unitOfWork.Repository<MaintenacePreventive>().GetByAsync(request.Id);
            Console.WriteLine(MaintPrev);
            if (MaintPrev != null)
            {
                MaintPrev.MachineId = request.MachineId;
                MaintPrev.Plan = request.Plan;
                MaintPrev.Actual = request.Actual;
                MaintPrev.StartDate = request.StartDate;
                MaintPrev.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<MaintenacePreventive>().UpdateAsyncById(MaintPrev, request.Id);
                MaintPrev.AddDomainEvent(new MaintPreventiveUpdateOkEvent(MaintPrev));
                var maintPrevDtoOk = _mapper.Map<UpdateMaintPreventiveDto>(MaintPrev);
                var machine = _unitOfWork.Repository<Machine>().Entities
                    .Where(x => x.Id == maintPrevDtoOk.MachineId)
                    .FirstOrDefault();
                var data = new UpdateMaintPreventiveDto()
                {
                    Id = maintPrevDtoOk.Id,
                    MachineId = machine.Id,
                    Name = machine.Name,
                    Plan = maintPrevDtoOk.Plan,
                    StartDate = (DateOnly)maintPrevDtoOk.StartDate,
                    Actual = maintPrevDtoOk.Actual,
                    EndDate = maintPrevDtoOk.EndDate,
                    ok = maintPrevDtoOk.EndDate == null ? false : true,
                };
                await _unitOfWork.Save(cancellationToken);
                return await Result<UpdateMaintPreventiveDto>.SuccessAsync(data, "Maintenance Preventive Updated");
            }
            return await Result<UpdateMaintPreventiveDto>.FailureAsync("Maintenance Preventive Not Found");
        }
    }
}
