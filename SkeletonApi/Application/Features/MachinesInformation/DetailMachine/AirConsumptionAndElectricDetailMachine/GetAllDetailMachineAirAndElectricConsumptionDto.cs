using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine
{
    public class GetAllDetailMachineAirAndElectricConsumptionDto : IMapFrom<GetAllDetailMachineAirAndElectricConsumptionDto>
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
        public List<DataAir> Data { get; set; }
    }
    public class DataAir
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}
