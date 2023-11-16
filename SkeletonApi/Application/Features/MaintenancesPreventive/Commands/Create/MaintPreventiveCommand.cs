using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
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
using System.Xml.Linq;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Create
{
    public record CreateMaintPreventiveCommand : IRequest<Result<CreateMaintPreventiveDto>>, IMapFrom<MaintenacePreventive>
    {
        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        [JsonPropertyName("plan")]
        public string Plan { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("machine_id")]
        public Guid? MachineId { get; set; }
    }
    internal class CreateMaintPreventiveCommandHandler : IRequestHandler<CreateMaintPreventiveCommand, Result<CreateMaintPreventiveDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMaintenancesPreventive _maintenancesPreventive;
        private readonly IMapper _mapper;

        public CreateMaintPreventiveCommandHandler(IUnitOfWork unitOfWork, IMaintenancesPreventive maintenancesPreventive, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _maintenancesPreventive = maintenancesPreventive;
            _mapper = mapper;
        }

        public async Task<Result<CreateMaintPreventiveDto>> Handle(CreateMaintPreventiveCommand request, CancellationToken cancellationToken)
        {
            // Periksa apakah Machine dengan ID yang diberikan ada
            var machine = await _unitOfWork.Repository<Machine>().Entities
                .Where(x => x.Id == request.MachineId)
                .FirstOrDefaultAsync(cancellationToken);

            if (machine != null)
            {
                var maintenance = new MaintenacePreventive()
                {
                    Id = Guid.NewGuid(),
                    Plan = request.Plan,
                    StartDate = request.StartDate,
                    MachineId = request.MachineId,
                    CreatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.Repository<MaintenacePreventive>().AddAsync(maintenance);
                maintenance.AddDomainEvent(new MaintPreventiveCreateEvent(maintenance));

                await _unitOfWork.Save(cancellationToken);

                var data = new CreateMaintPreventiveDto()
                {
                    Id = maintenance.Id,
                    MachineId = machine.Id,
                    Name = machine.Name,
                    Plan = maintenance.Plan,
                    StartDate = (DateOnly)maintenance.StartDate,
                    Actual = maintenance.Actual,
                    EndDate = maintenance.EndDate,
                    ok = maintenance.EndDate != null
                };

                return Result<CreateMaintPreventiveDto>.Success(data, "Maintenance Preventive Created");
            }
            else
            {
                return Result<CreateMaintPreventiveDto>.Failure("Machine Not Found");
            }
        }


    }
}
