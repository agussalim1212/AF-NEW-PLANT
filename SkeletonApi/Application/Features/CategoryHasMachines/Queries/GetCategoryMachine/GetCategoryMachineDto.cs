using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.CategoryHasMachines.Queries.GetCategoryMachine
{
    public class GetCategoryMachineDto : IMapFrom<GetCategoryMachineDto>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}