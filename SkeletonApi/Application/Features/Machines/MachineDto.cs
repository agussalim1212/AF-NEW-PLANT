using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Machines
{
    public record MachineDto
    {
        public string Name { get; init; }
    }
    public sealed record CreateMachineResponseDto : MachineDto { }
}
