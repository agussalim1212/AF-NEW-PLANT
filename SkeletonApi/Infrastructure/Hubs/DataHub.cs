using Microsoft.AspNetCore.SignalR;
using SkeletonApi.Infrastructure.DTOs;
using SkeletonApi.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SkeletonApi.Infrastructure.Hubs
{
    public class DataHub : Hub<IDataHub>
    {
        private readonly Services.MqttClientService _mqttClientService;

        public DataHub(Services.MqttClientService mqttClientService)
        {
           _mqttClientService = mqttClientService;
        }

        //public ChannelReader<IEnumerable<MqttClientService>> RealtimeMachine()
        //{
        //    //return _mqttClientService.Observe()
        //}
    }
}
