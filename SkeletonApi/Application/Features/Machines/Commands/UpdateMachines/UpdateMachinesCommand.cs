using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Machines.Commands.UpdateMachines
{
    public record UpdateMachinesCommand : IRequest<Result<Machine>>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("machine")]
        public string Name { get; set; }
 
    }

    internal class UpdateMachinesCommandHandler : IRequestHandler<UpdateMachinesCommand, Result<Machine>> 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMachinesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {   
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Machine>> Handle(UpdateMachinesCommand request, CancellationToken cancellationToken)
        {
            var machines = await _unitOfWork.Repository<Machine>().GetByIdAsync(request.Id);
            Console.WriteLine(machines);
            if (machines != null)
            {

                machines.Name = request.Name;
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
