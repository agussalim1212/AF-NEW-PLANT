using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Machines.Commands.UpdateMachines
{
   
    internal class UpdateMachinesCommandHandler : IRequestHandler<UpdateMachineRequest, Result<Machine>> 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMachinesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {   
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Machine>> Handle(UpdateMachineRequest request, CancellationToken cancellationToken)
        {
            var machines = await _unitOfWork.Repository<Machine>().GetByIdAsync(request.Id);
            Console.WriteLine(machines);
            if (machines != null)
            {
                machines.Vid = request.Vid;
                machines.Name = request.Machine;
                machines.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<Machine>().UpdateAsync(machines);
                machines.AddDomainEvent(new MachinesUpdateEvent(machines));

                await _unitOfWork.Save(cancellationToken);
                return await Result<Machine>.SuccessAsync(machines, "Machines Updated");
            }
            return await Result<Machine>.FailureAsync("Machine Not Found");
        }
    }
}
