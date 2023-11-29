using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.AirConsumption
{
    public class GetAllAirConsumptionDto : IMapFrom<GetAllAirConsumptionDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
        [JsonPropertyName("data")]
        public List<AirDto> Data { get; set; }
       

    }

    public class AirDto : IMapFrom<GetAllAirConsumptionDto>
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}
