using MediatR;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Subjects.Commands.CreateSubject
{
    public sealed record CreateSubjectRequest : IRequest<Result<CreateSubjectResponseDto>>
    {
        [JsonPropertyName("vid")]
        public string Vid { get; set; }
        [JsonPropertyName("subject")]
        public string Subjects { get; set; }

    }
}
