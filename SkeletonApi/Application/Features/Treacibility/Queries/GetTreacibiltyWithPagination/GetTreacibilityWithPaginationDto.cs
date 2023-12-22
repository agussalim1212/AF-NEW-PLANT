using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Tsdb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetSubjectWithPagination
{
    public class GetTreacibilityWithPaginationDto : IMapFrom<EnginePart>
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

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}
