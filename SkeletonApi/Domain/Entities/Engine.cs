using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SkeletonApi.Domain.Common.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkeletonApi.Domain.Entities
{
    public class Engine : BaseAuditableEntity
    {
        [Column("engine_id")]
        public string EngineId { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }
    }
}