using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Machines.Commands.DeleteMachines
{
    internal class DeleteMachinesCommandHandle : IRequestHandler<DeleteMachineRequest, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMachinesCommandHandle(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteMachineRequest request, CancellationToken cancellationToken)
        {
            var Machine = await _unitOfWork.Repository<Machine>().GetByIdAsync(request.Id);
            if (Machine != null)
            {
                Machine.DeletedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<Machine>().UpdateAsync(Machine);
                Machine.AddDomainEvent(new MachinesDeletedEvent(Machine));
                await _unitOfWork.Save(cancellationToken);

                return await Result<Guid>.SuccessAsync(Machine.Id, "Machine Deleted.");
            }
            return await Result<Guid>.FailureAsync("Machine Not Found");
        }

    }
}
