using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class ActivityUser
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? LogType { get; set; }
        public DateTime DateTime { get; set; }
    }
}
