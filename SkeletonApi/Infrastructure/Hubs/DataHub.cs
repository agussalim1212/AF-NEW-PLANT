using Microsoft.AspNetCore.SignalR;

namespace SkeletonApi.Infrastructure.Hubs
{
    public class DataHub : Hub<IDataHub>
    {
        //private readonly Services.MqttClientService _mqttClientService;

        //public DataHub(Services.MqttClientService mqttClientService)
        //{
        //    _mqttClientService = mqttClientService;
        //}

        //public ChannelReader<IEnumerable<MqttClientService>> RealtimeMachine()
        //{
        //    //return _mqttClientService.Observe()
        //}
    }
}