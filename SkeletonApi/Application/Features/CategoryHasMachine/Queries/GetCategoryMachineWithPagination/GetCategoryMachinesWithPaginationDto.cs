using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachineWithPagination
{
    public class GetCategoryMachinesWithPaginationDto : IMapFrom<CategoryMachineHasMachine>
    {

        [JsonPropertyName("category_id")]
        public Guid CategoryMachineId { get; set; }
        [JsonPropertyName("category_machine")]
        public string CategoryName { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? UpdatedAt { get; set; }
        public List<MachineDto> Machine { get; set; }
    }

    public class MachineDto : IMapFrom<Machine>
    {
        [JsonPropertyName("machine_id")]
        public Guid Id { get; init; }
        [JsonPropertyName("machine_name")]
        public string Name { get; init; }
    }
}
