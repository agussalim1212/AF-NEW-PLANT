using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityNumbering
{
    public class GetListQualityNumberingDto
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("data_barcode")]
        public string DataBarcode { get; set; }
        [JsonPropertyName("data_scan_img")]
        public string DataScanImage { get; set; }
    }
}
