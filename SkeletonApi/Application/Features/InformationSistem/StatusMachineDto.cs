using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.InformationSistem
{
   public class StatusMachineDto
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        public int Number { get; init; }

        [JsonPropertyName("value")]
        public byte Value { get; init; }

        [JsonPropertyName("date_time")]
        public string DateTimeString { get; init; }

        [JsonIgnore]
        public DateTime? DateTime { get; init; }
    }
}
