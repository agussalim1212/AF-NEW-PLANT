using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Settings.Commands.CreateSetting
{
    public class SettingCreatedEvent : BaseEvent
    {
        public Setting Setting { get; set; }

        public SettingCreatedEvent(Setting setting)
        {
            Setting = setting;
        }
    }
}