using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Dummys.DummyDto
{
    public class DummyDto : IMapFrom<DummyDto>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}