using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Dashboard._5_Top_Machine_Maintenance.Queries
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
}
