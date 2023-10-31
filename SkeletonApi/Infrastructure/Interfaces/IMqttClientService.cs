using Microsoft.Extensions.Hosting;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Infrastructure.Interfaces
{
    public interface IMqttClientService : IHostedService 
    {
        Task PublishAsync(string topic, string payload);
    }
}
