using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.FrameNumbers.Queries.GetFrameNumberWithPagination
{
    public class GetFrameNumberWithPaginationDto : IMapFrom<GetFrameNumberWithPaginationDto>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("vid")]
        public string Vid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("last_created")]
        public DateTime? UpdatedAt { get; set; }
    }
}