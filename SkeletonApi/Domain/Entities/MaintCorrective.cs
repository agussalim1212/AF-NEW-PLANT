using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities
{
    public class MaintCorrective : BaseAuditableEntity
    {

        [Column("actual")]
        public string? Actual { get; set; }

        [Column("start_date")]
        public DateOnly? StartDate { get; set; }

        [Column("count_actual")]
        public decimal? CountActual { get; set; }

        [Column("end_date")]
        public DateOnly? EndDate { get; set; }

        [Column("category")]
        public string? Category { get; set; } = "Corrective";

        [Column("machine_id")]
        public Guid? MachineId { get; set; }

        [NotMapped]
        public string? Name { get; set; }

        [NotMapped]
        public Machine machine { get; set; }
    }
}
