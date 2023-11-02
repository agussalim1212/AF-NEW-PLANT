using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class CategoryMachineHasMachine : BaseManyToMany
    {

        [Column("category_machine_id")]
        public Guid CategoryMachineId { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }
        public CategoryMachines CategoryMachine { get; set; }

        [Column("machine_id")]
        public Guid MachineId { get; set; }
        [NotMapped]
        public string MachineName { get; set; }
        public Machine Machine { get; set; }

        [NotMapped]
        public List<Machine> Machines { get; set; }
       

    }
}
