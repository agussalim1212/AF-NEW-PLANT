using System.Text.Json.Serialization;

namespace SkeletonApi.IotHub.Model
{
    public class MachineHealthModel
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("value")]
        public int Value { get; init; }

        [JsonPropertyName("date_time")]
        public string DateTimeString { get; init; }

        [JsonIgnore]
        public DateTime? Datetime { get; init; }

        //public MachineHealthModel()
        //{
        //    return null;
        //}
    }
}