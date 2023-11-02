using AutoMapper;
using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.Subjects.Commands.CreateSubject;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Machines.Commands.CreateMachines
{
    public record CreateMachinesCommand : IRequest<Result<Machine>>, IMapFrom<Machine>
    {
        [JsonPropertyName("machine")]
        public string Name { get; set; }
       
    }
    internal class CreateMachinesCommandHandler : IRequestHandler<CreateMachinesCommand, Result<Machine>>
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

        public async Task<Result<Machine>> Handle(CreateMachinesCommand request, CancellationToken cancellationToken)
        {
            var machines = new Machine()
            {
                
                Name = request.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            
            await _unitOfWork.Repository<Machine>().AddAsync(machines);
            machines.AddDomainEvent(new MachinesCreatedEvent(machines));
            await _unitOfWork.Save(cancellationToken);
            return await Result<Machine>.SuccessAsync(machines, "Machines Created");
        }
    }
}