using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
