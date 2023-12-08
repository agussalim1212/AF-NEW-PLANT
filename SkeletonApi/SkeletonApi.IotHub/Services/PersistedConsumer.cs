using AutoMapper;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
using System.Text.Json;
using SkeletonApi.IotHub.Services.Store;
using SkeletonApi.IotHub.Helpers;
using System.Globalization;

namespace SkeletonApi.IotHub.Services
{
    public class PersistedConsumer : BackgroundService
    {
        private readonly IIoTHubEventHandler<MqttRawDataEncapsulation> _mqttStoreEventHandler;
        private readonly IotHubMachineHealthEventHandler _machineHealthEventHandler;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly StatusMachineStore _StatusStore;

        public PersistedConsumer(IIoTHubEventHandler<MqttRawDataEncapsulation> mqttStoreEventHandler,
            IServiceScopeFactory serviceScopeFactory,
            IMapper mapper,
            IotHubMachineHealthEventHandler machineHealthEventHandler,
            StatusMachineStore machineStore
            )
        {
            _StatusStore = machineStore;
            _mqttStoreEventHandler = mqttStoreEventHandler;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _machineHealthEventHandler = machineHealthEventHandler;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            _mqttStoreEventHandler.Subscribe(
                subscriberName: typeof(PersistedConsumer).Name,
                action: async (val) =>
            {
                if (val.mqttRawData != null && val.topics != null)
                {
                    switch (val.topics)
                    {
                        case string a when a.Contains("MC-STATUS"):
                            await PersistMachineHealthToDBAsync(val.mqttRawData);
                            break;
                        default:
                            await Console.Out.WriteLineAsync("NULL");
                            break;
                    }
                }
            });

            return Task.CompletedTask;
        }

        private async Task PersistMachineHealthToDBAsync(MqttRawData value)
        {
            var mclist = _StatusStore.GetAllMachine();

            if (value.Values is not null)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var machineHealthList = from vls in value.Values
                                      join ids in mclist on vls.Vid equals ids.Vid
                                      where vls.Vid == ids.Vid
                                      group new { vls, ids } by vls.Vid into g
                                      orderby g.Key descending
                                      select new MachineHealthModel
                                      {
                                          Id = g.Key,
                                          Name = g.Last().ids.Name,
                                          Value = int.Parse(g.Last().vls.Value.ToString()),
                                          Datetime = DateTimeOffset.FromUnixTimeMilliseconds(g.Last().vls.Time).DateTime
                                      };
                    
                    _machineHealthEventHandler.Dispatch(machineHealthList);
                }
            }
        }
    }
}