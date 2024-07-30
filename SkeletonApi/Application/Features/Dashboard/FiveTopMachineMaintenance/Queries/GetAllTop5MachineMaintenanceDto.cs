using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Dashboard.FiveTopMachineMaintenance.Queries
{
    public class GetAllTop5MachineMaintenanceDto : IMapFrom<GetAllTop5MachineMaintenanceDto>
    {
        [JsonPropertyName("total_week")]
        public decimal TotalWeek { get; set; }

        [JsonPropertyName("total_month")]
        public decimal TotalMonth { get; set; }

        [JsonPropertyName("data_machine")]
        public List<DataMaintenance> DataMaintenance { get; set; }
    }

    public class DataMaintenance : IMapFrom<GetAllTop5MachineMaintenanceDto>
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }

    public record MaintenanceDto
    {
        public Guid MachineId { get; set; }
        public string MachineName { get; set; }
        public int Value { get; set; }
    }
}