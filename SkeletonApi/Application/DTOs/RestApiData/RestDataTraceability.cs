using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.DTOs.RestApiData
{
    public class RestDataTraceability
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName ("v")]
        public virtual object Value { get; set; }
        [JsonPropertyName("q")]
        public bool Quality { get; set; }
        [JsonPropertyName("t")]
        public long Time { get; set; }
    }
}
