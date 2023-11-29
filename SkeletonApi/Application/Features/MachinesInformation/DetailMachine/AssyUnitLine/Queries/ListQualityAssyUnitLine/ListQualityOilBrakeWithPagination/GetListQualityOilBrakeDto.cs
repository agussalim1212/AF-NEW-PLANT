using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrake
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
