using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
