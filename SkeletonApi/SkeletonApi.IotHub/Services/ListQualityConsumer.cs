using AutoMapper;
using Dapper;
using SkeletonApi.Application.Interfaces.Repositories.Configuration.Dapper;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
namespace SkeletonApi.IotHub.Services
{
    public class ListQualityConsumer : BackgroundService
    {
        private readonly IIoTHubEventHandler<MqttRawDataEncapsulation> _mqttStoreEventHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public ListQualityConsumer(IIoTHubEventHandler<MqttRawDataEncapsulation> mqttStoreEventHandler, IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _mqttStoreEventHandler = mqttStoreEventHandler;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _mqttStoreEventHandler.Subscribe(
                subscriberName: typeof(ListQualityConsumer).Name,
                action: async (val) =>
                {
                    if (val.mqttRawData != null && val.topics != null)
                    {
                        //Convert enumrable to list
                        List<MqttRawValue> mqttRawValues = val.mqttRawData.Values.AsList();
                        switch (val.topics)
                        {
                            
                            case string a when a.Contains("P9AUA0/BARCODE"):
                                //Remove value null or quality false
                                mqttRawValues.RemoveAll(x => x.Quality != true || x.Value == null);

                                if (val.mqttRawData.Values.Count(X => X.Vid.Contains("BARCODE")) > 0)
                                {
                                    await PersistListQualityDataBarcodeDBAsync(val.mqttRawData);
                                }
                                break;
                            default:
                            break;
                        }
                    }
                });

            return Task.CompletedTask;
        }
        private async Task PersistListQualityDataBarcodeDBAsync(MqttRawData value)
        {
            //await Console.Out.WriteLineAsync(JsonSerializer.Serialize(value));
            try
            {
                if (value.Values is not null)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scoped = scope.ServiceProvider.GetRequiredService<IEnginePartRepository>();
                        List<MqttRawValue> mqttRawValues = new List<MqttRawValue>();
                        foreach (var row in value?.Values)
                        {
                            mqttRawValues.Add(row);
                        }
                        var mqttRawValueEntities = _mapper.Map<IEnumerable<MqttRawValueEntity>>(mqttRawValues);
                        scoped.Create(mqttRawValueEntities);
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}
