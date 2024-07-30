using SkeletonApi.Domain.Common.Abstracts.Tsdb;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities.Tsdb
{
    public class ListQualityBarcode : TsdbEntity
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("data_barcode")]
        public string DataBarcode { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("foto_data_ng")]
        public string? FotoDataNg { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }
    }

    public record ListQualityBarcodeDto
    {
        public string Id { get; set; }
        public string DataBarcode { get; set; }
        public string? Status { get; set; }
        public string? FotoDataNg { get; set; }
        public DateTime DateTime { get; set; }
    }
}