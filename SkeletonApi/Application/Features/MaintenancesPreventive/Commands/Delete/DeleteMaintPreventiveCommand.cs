using AutoMapper;
using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.Machines.Commands.DeleteMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Delete
{
    public record DeleteMaintPreventiveCommand : IRequest<Result<Guid>>, IMapFrom<MaintenacePreventive>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        public DeleteMaintPreventiveCommand()
        {
            
        }

        public DeleteMaintPreventiveCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeleteMaintPreventiveCommandHandle : IRequestHandler<DeleteMaintPreventiveCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
       // private readonly IMaintenancesPreventive _maintenancesPreventive;
        private readonly IMapper _mapper;

        public DeleteMaintPreventiveCommandHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            //_maintenancesPreventive = maintenacePreventive;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(DeleteMaintPreventiveCommand command, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Repository<MaintenacePreventive>().GetByIdAsync(command.Id);
            if (account != null)
            {
                await _unitOfWork.Repository<MaintenacePreventive>().DeleteAsync(account);
                account.AddDomainEvent(new DeleteMaintPreventiveEvent(account));

                await _unitOfWork.Save(cancellationToken);

                return await Result<Guid>.SuccessAsync(account.Id, "Maintenance Preventive Deleted");
            }
            else
            {
                return await Result<Guid>.FailureAsync("Maintenance Preventive Not Found.");
            }
        }
    }
}
