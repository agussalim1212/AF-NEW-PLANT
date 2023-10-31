using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetSubjectWithPagination
{
    public class GetSubjectWithPaginationDto : IMapFrom<Subject>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("vid")]
        public string? Vid { get; set; }
        [JsonPropertyName("subject")]
        public string? Subjects { get; set; }
        [JsonPropertyName("last_created")]
        public string CreatedAt { get; set; }
    }
}
