using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling
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
