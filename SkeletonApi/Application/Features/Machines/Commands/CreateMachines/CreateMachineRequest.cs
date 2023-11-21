using MediatR;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Machines.Commands.CreateMachines
{
    public sealed record CreateMachineRequest : IRequest<Result<CreateMachineResponseDto>>
    {
        [JsonPropertyName("vid")]
        public string Vid { get; set; }
        [JsonPropertyName("machine")]
        public string Name { get; set; }
       
    }
}
