using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;


namespace SkeletonApi.Domain.Entities
{
    public class Setting : BaseAuditableEntity
    {

        [Column("machine_name")]
        public string MachineName { get; set; }

        [Column("subject_name")]
        public string SubjectName { get; set; }

        [Column("minimum")]
        public decimal? Minimum { get; set; }

        [Column("medium")]
        public decimal? Medium { get; set; }

        [Column("maximum")]
        public decimal? Maximum { get; set; }
    }
}
