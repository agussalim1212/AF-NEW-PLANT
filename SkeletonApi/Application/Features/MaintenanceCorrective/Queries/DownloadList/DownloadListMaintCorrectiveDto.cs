using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Queries.DownloadList
{
    public class DownloadListMaintCorrectiveDto
    {
        [JsonPropertyName("name")]
        [NotMapped] public string? Name { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly? StartDate { get; set; }

        [JsonPropertyName("actual")]
        public string? Actual { get; set; }

        [JsonPropertyName("end_date")]
        public DateOnly? EndDate { get; set; }
    }
}