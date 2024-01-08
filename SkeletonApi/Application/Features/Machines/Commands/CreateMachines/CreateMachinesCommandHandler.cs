using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Machines.Commands.CreateMachines
{
    internal class CreateMachinesCommandHandler : IRequestHandler<CreateMachineRequest, Result<CreateMachineResponseDto>>   
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMachinesRepository _machinesRepository;
        private readonly IMapper _mapper;

        public CreateMachinesCommandHandler(IUnitOfWork unitOfWork, IMachinesRepository machinesRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _machinesRepository = machinesRepository;
            _mapper = mapper;
        }

        public async Task<Result<CreateMachineResponseDto>> Handle(CreateMachineRequest request, CancellationToken cancellationToken)
        {
            var Machine = _mapper.Map<Machine>(request);
            var subjectMachineResponse = _mapper.Map<CreateMachineResponseDto>(Machine);
            var validateData = await _machinesRepository.ValidateData(Machine);

            if (validateData != true)
            {
                return await Result<CreateMachineResponseDto>.FailureAsync(subjectMachineResponse,"Data already exist");
            }

            Machine.CreatedAt = DateTime.UtcNow;
            Machine.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Machine>().AddAsync(Machine);
            Machine.AddDomainEvent(new MachinesCreatedEvent(Machine));
            await _unitOfWork.Save(cancellationToken);
            return await Result<CreateMachineResponseDto>.SuccessAsync(subjectMachineResponse, "Machine created.");

        }
    }
}