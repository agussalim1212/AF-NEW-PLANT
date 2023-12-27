using SkeletonApi.Domain.Common.Abstracts.Tsdb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities.Tsdb
{
    public class ListQuality : TsdbEntity
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("value")]
        public string Value { get; set; }

        [Column("quality")]
        public bool Quality { get; set; }

        [Column("time")]
        public long Time { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }
    }
}
