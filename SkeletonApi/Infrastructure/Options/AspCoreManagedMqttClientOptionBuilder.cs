﻿using MQTTnet.Extensions.ManagedClient;

namespace SkeletonApi.Infrastructure.Options
{
    public class AspCoreManagedMqttClientOptionBuilder : ManagedMqttClientOptionsBuilder
    {
        public IServiceProvider ServiceProvider { get; }

        public AspCoreManagedMqttClientOptionBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
