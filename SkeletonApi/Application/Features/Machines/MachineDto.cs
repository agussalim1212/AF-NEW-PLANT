namespace SkeletonApi.Application.Features.Machines
{
    public record MachineDto
    {
        public string Vid { get; init; }
        public string Name { get; init; }
    }
    public sealed record CreateMachineResponseDto : MachineDto { }
}