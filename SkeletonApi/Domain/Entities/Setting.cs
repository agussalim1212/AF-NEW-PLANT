using SkeletonApi.Domain.Common.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class Setting : BaseAuditableEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

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
