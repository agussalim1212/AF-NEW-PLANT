using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries
{
    public class GetAllDetailEnergyConsumptionDto : IMapFrom<GetAllDetailEnergyConsumptionDto>
    {
        [JsonPropertyName("value_kwh")]
        public decimal ValueKwh { get; set; }
        [JsonPropertyName("value_co2")]
        public decimal ValueCo2 { get; set; }
        [JsonIgnore]
        [JsonPropertyName("maximum")]
        public decimal? Maximum { get; set; }
        [JsonIgnore]
        [JsonPropertyName("medium")]
        public decimal? Medium { get; set; }
        [JsonIgnore]
        [JsonPropertyName("minimum")]
        public decimal? Minimum { get; set; }
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }

}
