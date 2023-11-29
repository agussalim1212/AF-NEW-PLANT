using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine
{
    public class GetAllEnergyConsumptionAssyWheelLineDto : IMapFrom<GetAllEnergyConsumptionAssyWheelLineDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
        [JsonPropertyName("data")]
        public List<EnergyAssyDto> Data { get; set; }

    }

    public class EnergyAssyDto : IMapFrom<GetAllEnergyConsumptionAssyWheelLineDto>
    {
        [JsonPropertyName("value_kwh")]
        public decimal ValueKwh { get; set; }
        [JsonPropertyName("value_co2")]
        public decimal ValueCo2 { get; set; }
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}
