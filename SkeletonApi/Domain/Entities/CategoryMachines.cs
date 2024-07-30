using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Domain.Entities
{
    public class CategoryMachines : BaseAuditableEntity
    {
        [Column("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<CategoryMachineHasMachine> CategoryMachineHasMachines { get; set; } = new List<CategoryMachineHasMachine>();
    }
}