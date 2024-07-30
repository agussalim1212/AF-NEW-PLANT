using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Notification.Queries.GetListNotif
{
    public class GetListNotifDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }

        [JsonPropertyName("date_time")]
        public string DateTime { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }
}