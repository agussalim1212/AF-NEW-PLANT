using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Machines.Commands.DeleteMachines
{
    public record DeleteMachineRequest : IRequest<Result<Guid>>, IMapFrom<Machine>
    {
        public Guid Id { get; set; }

        public DeleteMachineRequest(Guid id)
        {
            Id = id;
        }

        public DeleteMachineRequest()
        {
        }
    }
}
