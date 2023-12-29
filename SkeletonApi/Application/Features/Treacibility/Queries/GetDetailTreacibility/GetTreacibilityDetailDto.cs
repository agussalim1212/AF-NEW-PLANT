using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Tsdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Treacibility.Queries.GetDetailTreacibility
{
    public class GetTreacibilityDetailDto : IMapFrom<EnginePart>
    {
        [JsonPropertyName("engine_id")]
        public string EngineId { get; set; }

        [JsonPropertyName("torsi")]
        public string? Torsi { get; set; }

        [JsonPropertyName("abs")]
        public string? abs { get; set; }

        [JsonPropertyName("foto_data_ng")]
        public string? FotoDataNG { get; set; }

        [JsonPropertyName("oil_brake")]
        public string? OilBrake { get; set; }

        [JsonPropertyName("coolant")]
        public string? Coolant { get; set; }

    }
} 