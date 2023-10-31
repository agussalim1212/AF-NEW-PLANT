using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class Account : BaseAuditableEntity
    {
        public string Name { get; set; }
        public int NoNRP { get; set; }
        public string PhotoURL { get; set; }
        public DateTime? BirthDate { get; set; }
        public Guid? ClubId { get; set; }

    }
}
