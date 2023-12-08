using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetAllUsername
{
    public class GetAllUsernameDto : IMapFrom<GetAllUsernameDto>
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }    
    }
}
