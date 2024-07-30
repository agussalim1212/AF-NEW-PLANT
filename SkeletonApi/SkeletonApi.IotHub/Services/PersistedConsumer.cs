using AutoMapper;
using Dapper;
using SkeletonApi.Application.Interfaces.Repositories.Configuration.Dapper;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
using SkeletonApi.IotHub.Services.Store;
using System.Text.Json;

namespace SkeletonApi.IotHub.Services
{
    public class PersistedConsumer : BackgroundService
    {
        private readonly IIoTHubEventHandler<MqttRawDataEncapsulation> _mqttStoreEventHandler;
        private readonly IotHubMachineHealthEventHandler _machineHealthEventHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly StatusMachineStore _StatusStore;
        private readonly IMqttClientService _mqttClientService;

        public PersistedConsumer(IIoTHubEventHandler<MqttRawDataEncapsulation> mqttStoreEventHandler,
            IServiceScopeFactory serviceScopeFactory,
            IMapper mapper,
            IotHubMachineHealthEventHandler machineHealthEventHandler,
            StatusMachineStore machineStore,
            MqttClientServiceProvider provider
            )
        {
            _StatusStore = machineStore;
            _mqttStoreEventHandler = mqttStoreEventHandler;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _machineHealthEventHandler = machineHealthEventHandler;
            _mqttClientService = provider.MqttClientService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _mqttStoreEventHandler.Subscribe(
                subscriberName: typeof(PersistedConsumer).Name,
                action: async (val) =>
            {
                if (val.mqttRawData != null && val.topics != null)
                {
                    //Convert enumrable to list
                    List<MqttRawValue> mqttRawValues = val.mqttRawData.Values.AsList();
                    switch (val.topics)
                    {
                        case string a when a.Contains("OCR"):
                            await PersistTraceabilityToDBAsync(val.mqttRawData);
                            break;

                        //case string a when a.Contains("testing"):
                        //    await PersistTestingPublishAsync(val.mqttRawData);
                        //    break;
                        //case string a when a!.Contains("OCR") && a!.Contains("MC-STATUS"):
                        //    await PersistDeviceDataToDBAsync(val.mqttRawData);
                        //    break;
                        case string a when a.Contains("P9AUA0"):
                            //Remove value null or quality false
                            mqttRawValues.RemoveAll(x => x.Quality != true || x.Value == null);


                            if (val.mqttRawData.Values.Count(X => X.Vid.Contains("STATUS")) > 0)
                            {
                                //await PersistTestingPublishAsync(val.mqttRawData);
                                await PersistMachineHealthToDBAsync(val.mqttRawData);
                                await PersistDeviceDataToDBAsync(val.mqttRawData);
                                //await PersistListQualityDataBarcodeDBAsync(val.mqttRawData);
                            }
                            //if (val.mqttRawData.Values.Count(X => X.Vid.Contains("DATA-BARCODE")) > 0)
                            //{
                            //    await PersistListQualityAutoQGateToDBAsync(val.mqttRawData);
                            //}

                            break;

                        default:

                            break;
                    }
                }
            });

            return Task.CompletedTask;
        }

        private async Task PersistMachineHealthToDBAsync(MqttRawData value)
        {
            try
            {
                var mclist = _StatusStore.GetAllMachine();

                if (value.Values is not null)
                {
                    //var data = value.Values.Where(x => x.Vid.Contains("STATUS")).ToList();
                    //foreach (var m in data)
                    //{
                    //    int luc = Convert.ToInt32(m.Value.ToString());
                    //}
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var machineHealthList = from vls in value.Values.Where(x => x.Vid.Contains("STATUS") && x.Quality == true)
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
                        machineHealthList.OrderBy(x => x.Name).ToList();
                        _machineHealthEventHandler.Dispatch(machineHealthList);
                    }
                }

            }
            catch (Exception x)
            {
                await Console.Out.WriteLineAsync(x.Message);
            }
        }

        public async Task PersistTestingPublishAsync(MqttRawData value)
        {
            //await Console.Out.WriteLineAsync(JsonSerializer.Serialize(value));
            if (value.Values is not null)
            {
                try
                {
                    string jsonPayload = JsonSerializer.Serialize(value);
                    await _mqttClientService.PublishAsync("DCM/test", jsonPayload);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }
            }
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

        private async Task PersistTraceabilityToDBAsync(MqttRawData value)
        {
            //await Console.Out.WriteLineAsync(JsonSerializer.Serialize(value));
            if (value.Values is not null)
            {
                try
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
                        scoped.Creates(mqttRawValueEntities);
                    }
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }
            }
        }

        private async Task PersistDeviceDataToDBAsync(MqttRawData value)
        {
            //await Console.Out.WriteLineAsync(JsonSerializer.Serialize(value));
            if (value.Values is not null)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scoped = scope.ServiceProvider.GetRequiredService<IDiviceDateRepository>();
                        List<MqttRawValue> mqttRawValues = new List<MqttRawValue>();
                        foreach (var row in value?.Values)
                        {
                            mqttRawValues.Add(row);
                        }
                        var mqttRawValueEntities = _mapper.Map<IEnumerable<MqttRawValueEntity>>(mqttRawValues);
                        scoped.Creates(mqttRawValueEntities);
                    }
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }
            }
        }
    }
}