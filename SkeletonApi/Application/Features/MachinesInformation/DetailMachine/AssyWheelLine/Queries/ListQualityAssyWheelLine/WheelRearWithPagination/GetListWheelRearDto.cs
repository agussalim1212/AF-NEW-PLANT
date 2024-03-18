using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination
{
    public class GetListWheelRearDto
    {
        [JsonPropertyName("date_time")]
        public string? DateTime { get; set; }
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("data_distance")]
        public string? DataDistance { get; set; }
        [JsonPropertyName("data_tonase")]
        public string? DataTonase { get; set; }
        [JsonPropertyName("data_dial_horizontal")]
        public string? DataDialHorizontal { get; set; }
        [JsonPropertyName("data_dial_vertical")]
        public string? DataDialVertical { get; set; }
        [JsonPropertyName("data_dial_disk_brake")]
        public string? DiskBrake { get; set; } 
        [JsonPropertyName("tire_presure")]
        public string? TirePresure { get; set; }
        [JsonPropertyName("data_torsi")]
        public string? DataTorQ { get; set; }
    }

    

}
