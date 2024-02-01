using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.AirConsumptionAssyWheelLine
{
    public class GetAllAirConsumptionAssyWheelLineDto : IMapFrom<GetAllAirConsumptionAssyWheelLineDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
        [JsonPropertyName("maximum")]
        public decimal? Maximum { get; set; }
        [JsonPropertyName("medium")]
        public decimal? Medium { get; set; }
        [JsonPropertyName("minimum")]
        public decimal? Minimum { get; set; }
        [JsonPropertyName("data")]
        public List<AirAssyWheelDto> Data { get; set; }


    }

    public class AirAssyWheelDto : IMapFrom<GetAllAirConsumptionAssyWheelLineDto>
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}
