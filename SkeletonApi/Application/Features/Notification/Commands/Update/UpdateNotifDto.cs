using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Notification.Commands.Update
{
    public class UpdateNotifDto : IMapFrom<Notifications>
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }
}