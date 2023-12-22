using SkeletonApi.Domain.Common.Abstracts.Tsdb;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Domain.Entities.Tsdb
{
    public class EnginePart : TsdbEntity
    {
        [Column("engine_id")]
        public string EngineId { get; set; }

        [Column("torsi")]
        public string? Torsi { get; set; }

        [Column("abs")]
        public string? abs { get; set; }

        [Column("foto_data_ng")]
        public string? FotoDataNG { get; set; }

        [Column("oil_brake")]
        public string? OilBrake { get; set; }

        [Column("coolant")]
        public string? Coolant { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }
    }

    public class MqttRawDataEntity : TsdbEntity
    {
        [JsonPropertyName("timestamp")]
        public virtual object timestamp { get; init; }

        [JsonPropertyName("values")]
        public IEnumerable<MqttRawValueEntity> Values { get; init; }
    }

    public class EnginePartRaw : TsdbEntity
    {
        //[Column("engine_id")]
        public string masterPartId { get; set; }

        //[Column("cr_cs_l")]
        public string partId1 { get; set; }

        public string? partId2 { get; set; }

        //[Column("date_time")]
        public DateTime Datetime { get; set; }
    }

    public class EnginePartForUpdate : TsdbEntity
    {
        //[Column("engine_id")]
        public string EngineId { get; set; }

        //[Column("cr_cs_l")]
        public string partId { get; set; }

        public string partId2 { get; set; }

        //[Column("date_time")]
        public DateTime DateTime { get; set; }
    }

    public record MqttRawValueEntity
    {
        public string Vid { get; init; }
        public virtual object Value { get; init; }
        public bool Quality { get; init; }

        public long Time { get; init; }

        public DateTime Datetime { get; init; }
    }

    public enum StatusEngine
    {
        [Display(Name = "OK")]
        OK,

        [Display(Name = "Repair")]
        RP
    }

    public class EnginePartDto
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