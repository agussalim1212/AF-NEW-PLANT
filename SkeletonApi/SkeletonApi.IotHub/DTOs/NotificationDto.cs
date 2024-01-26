using System.Text.Json.Serialization;

namespace SkeletonApi.IotHub.DTOs
{
    public class NotificationDto
    {

        [JsonPropertyName("machine_name")]
        public string? MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string? SubjectName { get; set; }
        public decimal? Minimum { get; set; }
        public decimal? Medium { get; set; }
        public decimal? Maximum { get; set; }
    }
}
