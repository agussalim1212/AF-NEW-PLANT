using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetAllLogType
{
    public class GetAllLogTypeDto : IMapFrom<GetAllLogTypeDto>
    {
        [JsonPropertyName("log_type")]
        public string logType { get; set; }
    }
}
