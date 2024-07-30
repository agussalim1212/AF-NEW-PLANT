using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityDataBarcodeWithPagination
{
    public class GetListQualityBarcodeDto
    {
        [JsonPropertyName("date_time")]
        public DateTime? DateTime { get; set; }
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("data_barcode")]
        public string? DataBarcode { get; set; }
        [JsonPropertyName("foto_data_ng")]
        public string? FotoDataNg { get; set; }

    }
}