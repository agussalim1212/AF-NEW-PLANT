using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.CategoryHasMachines.Queries.GetAllCategoryMachine
{
    public class GetAllCategoryMachineDto : IMapFrom<GetAllCategoryMachineDto>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("category")]
        public string CategoryName { get; set; }
    }
}