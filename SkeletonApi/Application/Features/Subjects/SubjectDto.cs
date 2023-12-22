using System.Text.Json.Serialization;


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
    