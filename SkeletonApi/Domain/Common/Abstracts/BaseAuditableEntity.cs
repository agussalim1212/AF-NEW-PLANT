using SkeletonApi.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Common.Abstracts
{
    public abstract class BaseAuditableEntity : BaseEntity,IAuditableEntity
    {
        [Column("created_by")]
        public Guid? CreatedBy { get; set; }
        [Column("update_by")]
        public Guid? UpdatedBy { get; set; }
        [Column("deleted_by")]
        public Guid? DeletedBy { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } 
        [Column("update_at")]
        public DateTime? UpdatedAt { get; set; }
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}
