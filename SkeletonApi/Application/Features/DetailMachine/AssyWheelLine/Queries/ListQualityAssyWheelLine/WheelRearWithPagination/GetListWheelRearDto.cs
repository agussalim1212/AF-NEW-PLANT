using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination
{
    public class GetListWheelRearDto
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("data_dial")]
        public decimal DataDial { get; set; }
        [JsonPropertyName("data_tonase")]
        public decimal DataTonase { get; set; }
        [JsonPropertyName("data_dial_horizontal")]
        public decimal DataDialHorizontal { get; set; }
        [JsonPropertyName("data_dial_vertical")]
        public decimal DataDialVertical { get; set; }
        [JsonPropertyName("tire_presure")]
        public decimal TirePresure { get; set; }
        [JsonPropertyName("data_torsi")]
        public decimal DataTorQ { get; set; }
    }
}
