using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Shared
{
    public class PaginatedResult<T> : Result<T>
    {
        public PaginatedResult()
        {
            
        }

        public PaginatedResult(List<T> data)
        {
            Data = data;
        }

        public PaginatedResult(bool succeeded, List<T> data = default, List<string> messages = null, int count = 0, int pageNumber = 1, int pageSize = 10)
        {
            Data = data;
            page_number = pageNumber;
            Status = succeeded;
            Messages = messages;
            page_size = pageSize;
            total_pages = (int)Math.Ceiling(count / (double)pageSize);
            total_count = count;
        }

        public new List<T> Data { get; set; }
        [JsonPropertyName("page_number")]
        public int page_number { get; set; }
        [JsonPropertyName("total_pages")]
        public int total_pages { get; set; }
        [JsonPropertyName("page_size")]
        public int page_size { get; set; }
        [JsonPropertyName("total_count")]
        public int total_count { get; set; }

        [JsonPropertyName("has_previous")]
        public bool has_previous => page_number > 1;
        [JsonPropertyName("has_next")]
        public bool has_next => page_number < total_pages;




        public static PaginatedResult<T> Create(List<T> data, int count, int pageNumber, int pageSize)
        {
            return new PaginatedResult<T>(true, data, null, count, pageNumber, pageSize);

        }
     



    }
}
