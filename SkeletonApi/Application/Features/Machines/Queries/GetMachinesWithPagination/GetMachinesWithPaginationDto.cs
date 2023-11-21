using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Machines.Queries.GetAllMachines
{
    public class GetMachinesWithPaginationDto : IMapFrom<Machine>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("vid")]
        public string Vid { get; set; }
        [JsonPropertyName("machine")]
        public string Name { get; set; }
        [JsonPropertyName("last_created")]
        public DateTime? UpdatedAt { get; set; }

    }
}
