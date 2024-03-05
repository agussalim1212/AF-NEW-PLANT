using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.FrameNumberSubject.Queries.GetFrameNumberHasSubjectWithPagination
{
    public class GetFrameNumberHasSubjectWithPaginationDto : IMapFrom<GetFrameNumberHasSubjectWithPaginationDto>
    {
        [JsonPropertyName("frame_number_id")]
        public Guid FrameNumberId { get; set; }
        [JsonPropertyName("frame_number_name")]
        public string FrameNumberName { get; set; }
       
        [JsonPropertyName("last_created")]
        public DateTime UpdateAt { get; set; }
        [JsonPropertyName("data")]
        public List<SubjectsDto> Data { get; set; }

    }

    public class SubjectsDto : IMapFrom<Subject>
    {
        [JsonPropertyName("subject_id")]
        public Guid Id { get; init; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
    }

}
