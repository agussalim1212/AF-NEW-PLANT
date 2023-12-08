using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Dashboard._5_Top_Air_Consumptions.Queries
{
    public class GetAllTop5AirConsumptionsDto : IMapFrom<GetAllTop5AirConsumptionsDto>
    {
        [JsonPropertyName("total_week")]
        public decimal TotalWeek { get; set; }
        [JsonPropertyName("total_month")]
        public decimal TotalMonth { get; set; }
        [JsonPropertyName("data_machine")]
        public List<DataMachines> DataMachines { get; set; }
    }

    public class DataMachines : IMapFrom<GetAllTop5AirConsumptionsDto>
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }
}
