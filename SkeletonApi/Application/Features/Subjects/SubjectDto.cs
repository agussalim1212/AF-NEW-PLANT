using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Subjects
{
    public record SubjectDto
    {
        [JsonPropertyName("vid")]
        public string Vid { get; set; }
        [JsonPropertyName("subject")]
        public string Subjects { get; set; }
    }
    public sealed record CreateSubjectResponseDto : SubjectDto { }
}
