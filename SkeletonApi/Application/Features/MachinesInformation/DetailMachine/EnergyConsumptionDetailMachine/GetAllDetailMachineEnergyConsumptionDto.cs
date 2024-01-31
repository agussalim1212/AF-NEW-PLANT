using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption
{
    public class GetAllDetailMachineEnergyConsumptionDto : IMapFrom<GetAllDetailMachineEnergyConsumptionDto>
    {
        [JsonPropertyName("vid")]
        public string Vid { get; set; }
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
        public List<DataPower> Data { get; set; }
    }

    public class DataPower
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
