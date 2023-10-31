using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Shared
{
    public class MetaData
    {
        [JsonPropertyName("page_number")]
        public int PageNumber { get; set; }
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("has_previous")]
        public bool HasPrevious => PageNumber > 1;
        [JsonPropertyName("has_next")]
        public bool HasNext => PageNumber < TotalPages;
    }
}
