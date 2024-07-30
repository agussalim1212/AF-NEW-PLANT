using System.Text.Json.Serialization;

namespace SkeletonApi.Infrastructure.Models
{
    public class ModelMqttReceive
    {
        [JsonPropertyName("timestamp")]
        public virtual object timestamps { get; init; }

        [JsonPropertyName("values")]
        public IEnumerable<Values> Values { get; init; }
    }
}