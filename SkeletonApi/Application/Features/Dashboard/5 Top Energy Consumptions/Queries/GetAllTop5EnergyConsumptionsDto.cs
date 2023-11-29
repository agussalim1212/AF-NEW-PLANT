using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Dashboard._5_Top_Energy_Consumptions
{
    public class GetAllTop5EnergyConsumptionsDto : IMapFrom<GetAllTop5EnergyConsumptionsDto>
    {
        [JsonPropertyName("total_week")]
        public decimal TotalWeek { get; set; }
        [JsonPropertyName("total_month")]
        public decimal TotalMonth { get; set; }
        [JsonPropertyName("data_machine")]
        public List<DataMachine> DataMachines { get; set; }
    }

    public class DataMachine : IMapFrom<GetAllTop5EnergyConsumptionsDto>
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }
}