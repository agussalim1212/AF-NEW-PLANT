using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFilingWithPagination
{
    public class GetListQualityCoolantFilingDto : IMapFrom<Dummy>
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("volume_coolant")]
        public decimal VolumeCoolant { get; set; }

        [JsonPropertyName("data_barcode")]
        public string DataBarcode { get; set; }
    }
}