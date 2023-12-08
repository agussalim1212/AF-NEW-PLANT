using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    [Keyless]
    public class Dummy
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

        [NotMapped]
        public string DateTimeString { get; set; }
        [NotMapped]
        public Subject Subject { get; set; }
        [NotMapped]
        public Machine Machine { get; set; }
    }
}
