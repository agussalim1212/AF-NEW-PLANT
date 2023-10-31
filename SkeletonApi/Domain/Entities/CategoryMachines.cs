using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
