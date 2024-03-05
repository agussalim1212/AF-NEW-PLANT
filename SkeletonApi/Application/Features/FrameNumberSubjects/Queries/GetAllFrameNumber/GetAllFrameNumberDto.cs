using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.FrameNumberSubject.Queries.GetAllFrameNumber
{
    public class GetAllFrameNumberDto : IMapFrom<FrameNumber>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
