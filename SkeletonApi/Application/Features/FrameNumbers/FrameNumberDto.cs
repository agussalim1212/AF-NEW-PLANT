using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.FrameNumb
{
    public record FrameNumberDto
    {
        public string Vid { get; set; }
        public string Name { get; set; }
    }
    public sealed record CreateFrameNumberResponseDto : FrameNumberDto { }
}
