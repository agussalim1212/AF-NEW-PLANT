using AutoMapper;
using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Delete;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Delete
{
    public record DeleteMaintCorrectiveCommand : IRequest<Result<Guid>>, IMapFrom<MaintCorrective>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        public DeleteMaintCorrectiveCommand()
        {
            
        }

        public DeleteMaintCorrectiveCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeleteMaintCorrectiveCommandHandle : IRequestHandler<DeleteMaintCorrectiveCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteMaintCorrectiveCommandHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(DeleteMaintCorrectiveCommand command, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Repository<MaintCorrective>().GetByIdAsync(command.Id);
            if (account != null)
            {
                await _unitOfWork.Repository<MaintCorrective>().DeleteAsync(account);
                account.AddDomainEvent(new DeleteMaintCorrectiveEvent(account));

                await _unitOfWork.Save(cancellationToken);

                return await Result<Guid>.SuccessAsync(account.Id, "Maintenance Corrective Deleted");
            }
            else
            {
                return await Result<Guid>.FailureAsync("Maintenance Corrective Not Found.");
            }
        }
    }
}
