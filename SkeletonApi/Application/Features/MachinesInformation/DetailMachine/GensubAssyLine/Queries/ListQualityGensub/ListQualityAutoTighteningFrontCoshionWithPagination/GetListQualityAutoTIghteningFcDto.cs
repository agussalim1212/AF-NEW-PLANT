using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityAutoTighteningFrontCoshionWithPagination
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
        public Decimal DataTorQ { get; set; }
    }
}