using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetAllUsername
{
    public class GetAllUsernameDto : IMapFrom<GetAllUsernameDto>
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}