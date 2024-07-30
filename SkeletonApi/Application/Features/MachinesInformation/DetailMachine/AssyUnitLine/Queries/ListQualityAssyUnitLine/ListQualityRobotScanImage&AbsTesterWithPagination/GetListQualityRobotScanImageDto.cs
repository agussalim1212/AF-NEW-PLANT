using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage
{
    public class GetListQualityRobotScanImageDto
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("data_barcode")]
        public string DataBarcode { get; set; }
    }
}