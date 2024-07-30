using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetActivityUserWithPagination
{
    public class GetActivityUserWithPaginationDto : IMapFrom<GetActivityUserWithPaginationDto>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("log_type")]
        public string LogType { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime Datetime { get; set; }
    }
}