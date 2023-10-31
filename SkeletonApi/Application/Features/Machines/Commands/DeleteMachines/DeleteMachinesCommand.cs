using AutoMapper;
using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.Subjects.Commands.DeleteSubject;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;

namespace SkeletonApi.Application.Features.Machines.Commands.DeleteMachines
{
    public record DeleteMachinesCommand : IRequest<Result<Guid>>, IMapFrom<Machine>
    {
        public Guid Id { get; set; }

        public DeleteMachinesCommand()
        {

        }
        public DeleteMachinesCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeleteMachinesCommandHandle : IRequestHandler<DeleteMachinesCommand, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMachinesCommandHandle(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteMachinesCommand request, CancellationToken cancellationToken)
        {
            var machines = await _unitOfWork.Repository<Machine>().GetByIdAsync(request.Id);

            if (machines != null)
            {
                machines.DeletedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<Machine>().DeleteAsync(machines);
                machines.AddDomainEvent(new MachinesDeletedEvent(machines));
                await _unitOfWork.Save(cancellationToken);

                return await Result<Guid>.SuccessAsync(machines.Id, "Machines Deleted");
            }

            return await Result<Guid>.FailureAsync("Machines not found");
        }

    }
}
