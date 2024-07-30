using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrakeWithPagination
{
    public class GetListQualityOilBrakeDto
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("data_barcode")]
        public string DataBarcode { get; set; }

        [JsonPropertyName("leak_tester")]
        public decimal LeakTester { get; set; }

        [JsonPropertyName("volume_oil_brake")]
        public decimal VolumeOilBrake { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }
    }
}