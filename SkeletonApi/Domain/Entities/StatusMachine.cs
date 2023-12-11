using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class StatusMachine
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("vid")]
        public string Vid { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("value")]
        public int Value { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }
    }
}
