using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityGensubWithPagination
{
    public class GetListQualityGensubDto : IMapFrom<GetListQualityGensubDto>
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}
