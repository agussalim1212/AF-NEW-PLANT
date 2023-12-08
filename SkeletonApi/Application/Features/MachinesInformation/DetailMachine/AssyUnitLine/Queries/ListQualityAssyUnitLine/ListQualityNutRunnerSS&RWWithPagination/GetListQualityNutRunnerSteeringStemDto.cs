using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination
{
    public class GetListQualityNutRunnerSteeringStemDto : IMapFrom<GetListQualityNutRunnerSteeringStemDto>
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("data_barcode")]
        public string DataBarcode { get; set; }
        [JsonPropertyName("data_torsi")]
        public decimal DataTorQ { get; set; }
    }

   
}
