namespace SkeletonApi.Application.Features.FrameNumbers
{
    public record FrameNumberDto
    {
        public string Vid { get; set; }
        public string Name { get; set; }
    }
    public sealed record CreateFrameNumberResponseDto : FrameNumberDto { }
}