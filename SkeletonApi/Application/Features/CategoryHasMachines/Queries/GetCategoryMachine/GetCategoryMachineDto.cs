using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryHasMachine.Queries.GetCategoryMachine
{
    public class GetCategoryMachineDto : IMapFrom<GetCategoryMachineDto>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
