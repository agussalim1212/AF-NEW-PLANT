using SkeletonApi.Domain.Common.Abstracts.Tsdb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities.Tsdb
{
    public class DeviceData : TsdbEntity
    {
        public string Id { get; set; }

        public string Value { get; set; }

        public bool Quality { get; set; }

        public long Time { get; set; }

        public DateTime DateTime { get; set; }
    }
}
