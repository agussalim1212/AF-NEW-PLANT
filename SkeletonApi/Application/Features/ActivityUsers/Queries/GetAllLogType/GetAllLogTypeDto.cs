using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetAllLogType
{
    public class GetAllLogTypeDto : IMapFrom<GetAllLogTypeDto>
    {
        [JsonPropertyName("log_type")]
        public string logType { get; set; }
    }
}