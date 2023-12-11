using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Notification.Commands.Update
{
    public class UpdateNotifDto : IMapFrom<Notifications>
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }
}
