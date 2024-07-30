using System.Text.Json.Serialization;

namespace SkeletonApi.IotHub.Model
{
    public class NotificationModel
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; init; }

        [JsonPropertyName("message")]
        public string Message { get; init; }

        [JsonPropertyName("date_time")]
        public string DateTimeString { get; init; }

        [JsonIgnore]
        public DateTime? Datetime { get; init; }

        public bool Status { get; init; }
    }
}