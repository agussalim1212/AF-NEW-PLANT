using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.FrameNumberSubjects.Queries.GetAllFrameNumber
{
    public class GetAllFrameNumberDto : IMapFrom<FrameNumber>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}