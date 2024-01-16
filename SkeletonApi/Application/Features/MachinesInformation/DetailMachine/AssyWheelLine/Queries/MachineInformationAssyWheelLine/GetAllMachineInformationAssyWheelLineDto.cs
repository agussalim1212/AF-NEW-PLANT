using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine
{
    public class GetAllMachineInformationAssyWheelLineDto : IMapFrom<GetAllMachineInformationAssyWheelLineDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("value_running")]
        public decimal ValueRunning { get; set; }
        [JsonPropertyName("value_cycle_count")]
        public decimal CycleCount { get; set; }
        [JsonPropertyName("value_last_time_calibration")]
        public string LastTimeCalibration { get; set; }

    }
}
