using Microsoft.EntityFrameworkCore;
using SkeletonApi.Domain.Common.Abstracts.Tsdb;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Domain.Entities.Tsdb
{
    [Keyless]
    public class TraceabilityResult : TsdbEntity
    {
        [Column("engine_id")]
        public string EngineId { get; set; }

        [Column("process_part")]
        public string ProcessPart { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }

        /* [NotMapped]
         public string DateTimeString { get; set; }*/
    }

    public class TraceabilityResultDto
    {
        [JsonPropertyName("engine_id")]
        public string EngineId { get; set; }

        [JsonPropertyName("process_part")]
        public string ProcessPart { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

        [JsonIgnore]
        public string DateTimeString { get; set; }
    }
}