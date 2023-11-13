using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Settings.Queries.GetSettingWithPagination
{
    public class GetSettingWithPaginationDto : IMapFrom<Setting>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }

        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }

        [JsonPropertyName("minimum")]
        public decimal? Minimum { get; set; }

        [JsonPropertyName("medium")]
        public decimal? Medium { get; set; }

        [JsonPropertyName("maximum")]
        public decimal? Maximum { get; set; }
        [JsonPropertyName("last_created")]
        public DateTime? UpdatedAt { get; set; }
    }
}
