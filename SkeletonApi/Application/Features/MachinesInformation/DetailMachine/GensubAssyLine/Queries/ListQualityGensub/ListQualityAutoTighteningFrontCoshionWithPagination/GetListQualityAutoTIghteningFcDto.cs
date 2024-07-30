using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityAutoTighteningFrontCoshionWithPagination
{
    public class GetListQualityAutoTIghteningFcDto
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