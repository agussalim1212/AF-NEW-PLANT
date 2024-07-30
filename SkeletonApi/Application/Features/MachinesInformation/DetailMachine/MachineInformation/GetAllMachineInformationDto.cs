using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.MachineInformation
{
    public class GetAllMachineInformationDto : IMapFrom<GetAllMachineInformationDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }

        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }

        [JsonPropertyName("value_running")]
        public string ValueRunning { get; set; }

        [JsonPropertyName("value_last_time_calibration")]
        public string LastTimeCalibration { get; set; }
    }
}