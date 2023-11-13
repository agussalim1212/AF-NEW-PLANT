using MediatR;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Machines.Commands.CreateMachines
{
    public sealed record CreateMachineRequest : IRequest<Result<CreateMachineResponseDto>>
    {
        public string Name { get; set; }
       
    }
}
