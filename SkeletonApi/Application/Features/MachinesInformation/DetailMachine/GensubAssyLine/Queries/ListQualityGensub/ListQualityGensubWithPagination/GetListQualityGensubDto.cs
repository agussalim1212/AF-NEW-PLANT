using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityGensubWithPagination
{
    public class GetListQualityGensubDto : IMapFrom<GetListQualityGensubDto>
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}