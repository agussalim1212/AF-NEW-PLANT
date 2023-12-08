using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
