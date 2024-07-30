using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction
{
    public class GetAllTotalProductionDto : IMapFrom<GetAllTotalProductionDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }

        [JsonPropertyName("value_ok_total")]
        public decimal ValueOkTotal { get; set; }

        [JsonPropertyName("value_ng_total")]
        public decimal ValueNgTotal { get; set; }

        [JsonPropertyName("value_ok_persentase")]
        public decimal ValueOKPresentase { get; set; }

        [JsonPropertyName("value_ng_persentase")]
        public decimal ValueNgPresentase { get; set; }
    }
}