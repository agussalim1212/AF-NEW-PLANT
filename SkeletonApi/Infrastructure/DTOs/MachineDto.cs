using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Infrastructure.DTOs
{
    public class MachineDto
    {
        public string id { get; set; }

        public string name { get; set; }

        public string category_machine { get; set; }

        public int value { get; set; }

        public string date_time { get; set; }
    }
}
