using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityPressConeRace
{
    public class GetListQualityPressConeRaceDto 
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("kedalaman")]
        public decimal Kedalaman { get; set; }
        [JsonPropertyName("tonase")]
        public decimal Tonase { get; set; }
    }
}
