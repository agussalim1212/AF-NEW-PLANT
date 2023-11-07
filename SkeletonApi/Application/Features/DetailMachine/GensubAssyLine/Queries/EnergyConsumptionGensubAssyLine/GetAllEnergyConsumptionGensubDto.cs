using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.EnergyConsumptionGensubAssyLine
{
    public class GetAllEnergyConsumptionGensubDto  : IMapFrom<GetAllEnergyConsumptionGensubDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
        [JsonPropertyName("data")]
        public List<EnergyGensubDto> Data { get; set; }

        [JsonIgnore]
       // [JsonPropertyName("value_kwh")]
        public decimal ValueKwh { get; set; }
        //  [JsonPropertyName("value_co2")]
        [JsonIgnore]
        public decimal ValueCo2 { get; set; }
        // [JsonPropertyName("label")]
        [JsonIgnore]
        public string Label { get; set; }
       // [JsonPropertyName("date_time")]
        [JsonIgnore]
        public DateTime DateTime { get; set; }
    }

    public class EnergyGensubDto : IMapFrom<GetAllEnergyConsumptionGensubDto>
    {
        [JsonPropertyName("value_kwh")]
        public decimal ValueKwh { get; set; }
        [JsonPropertyName("value_co2")]
        public decimal ValueCo2 { get; set; }
        [JsonPropertyName("label")]
        public string Label { get; set; }
        //[JsonPropertyName("date_time")]
        //public DateTime DateTime { get; set; }
    }
}
