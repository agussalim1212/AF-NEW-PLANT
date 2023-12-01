using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Infrastructure.Models
{
    public record Values
    {
        [JsonPropertyName("id")]
        public string Vid { get; init; }

        [JsonPropertyName("v")]
        public virtual object Value { get; init; }

        [JsonPropertyName("q")]
        public bool Quality { get; init; }

        [JsonPropertyName("t")]
        public long Time { get; init; }
    }
}
