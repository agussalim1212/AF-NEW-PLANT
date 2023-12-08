using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.StopLine
{
    public class GetAllStopLineDto : IMapFrom<GetAllStopLineDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("total_stop")]
        public int TotalStop { get; set; }
        [JsonPropertyName("stop_time")]
        public int StopTime { get; set; }
    }
}
