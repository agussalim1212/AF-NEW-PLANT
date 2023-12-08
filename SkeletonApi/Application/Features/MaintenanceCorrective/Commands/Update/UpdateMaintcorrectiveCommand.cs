using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Update;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Update
{
    public record UpdateMaintCorrectiveCommand : IRequest<Result<UpdateMaintCorrectiveDto>>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("machine_id")]
        public Guid? MachineId { get; set; }

        [JsonPropertyName("actual")]
        public string? Actual { get; init; }

        [JsonPropertyName("start_date")]
        public DateOnly? StartDate { get; init; }

        [JsonPropertyName("end_date")]
        public DateOnly? EndDate { get; init; }
    }
    internal class UpdateMaintCorrectiveCommandHandle : IRequestHandler<UpdateMaintCorrectiveCommand, Result<UpdateMaintCorrectiveDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMaintCorrectiveCommandHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UpdateMaintCorrectiveDto>> Handle(UpdateMaintCorrectiveCommand request, CancellationToken cancellationToken)
        {
            var MaintPrev = await _unitOfWork.Repository<MaintCorrective>().GetByAsync(request.Id);
            if (MaintPrev != null)
            {
                MaintPrev.MachineId = request.MachineId;
                MaintPrev.Actual = request.Actual;
                MaintPrev.StartDate = request.StartDate;
                MaintPrev.EndDate = request.EndDate;
                MaintPrev.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<MaintCorrective>().UpdateAsyncById(MaintPrev, request.Id);
                MaintPrev.AddDomainEvent(new UpdateMaintCorrectiveEvent(MaintPrev));
                var maintPrevDtoOk = _mapper.Map<UpdateMaintCorrectiveDto>(MaintPrev);
                var machine = _unitOfWork.Repository<Machine>().Entities
                    .Where(x => x.Id == maintPrevDtoOk.MachineId)
                    .FirstOrDefault();
                var data = new UpdateMaintCorrectiveDto()
                {
                    Id = maintPrevDtoOk.Id,
                    MachineId = machine.Id,
                    Name = machine.Name,
                    StartDate = (DateOnly)maintPrevDtoOk.StartDate,
                    Actual = maintPrevDtoOk.Actual,
                    EndDate = maintPrevDtoOk.EndDate,
                };
                await _unitOfWork.Save(cancellationToken);
                await _unitOfWork.Save(cancellationToken);
                return await Result<UpdateMaintCorrectiveDto>.SuccessAsync(maintPrevDtoOk, "Maintenance Corrective Updated");
            }
            return await Result<UpdateMaintCorrectiveDto>.FailureAsync("Maintenance Corrective Not Found");
        }
    }
}
