using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;


namespace SkeletonApi.Domain.Entities
{
    public class Notifications : BaseAuditableEntity
    {

        [Column("machine_name")]
        public string MachineName { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("status")]
        public bool Status { get; set; }
    }
}
