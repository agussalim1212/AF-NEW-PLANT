using SkeletonApi.Domain.Common.Abstracts.Tsdb;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities.Tsdb
{
    public class MachineActivity : TsdbEntity
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("value")]
        public int Value { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }

        [NotMapped]
        public string DateTimeString { get; set; }
    }
}