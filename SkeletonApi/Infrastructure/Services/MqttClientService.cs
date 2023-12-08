using Microsoft.AspNetCore.SignalR;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkeletonApi.Infrastructure.Interfaces;
using SkeletonApi.Infrastructure.Hubs;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.Design;
using SkeletonApi.Infrastructure.Helpers;
using SkeletonApi.Infrastructure.Models;
using SkeletonApi.Infrastructure.DTOs;
using System.Globalization;

namespace SkeletonApi.Infrastructure.Services
{
    public class MqttClientService : IMqttClientService
    {
        private readonly IManagedMqttClient _managedMqttClient;
        private readonly ManagedMqttClientOptions options;
        private readonly ILogger<MqttClientService> _logger;
        private readonly IConfiguration _configuration; 

        Tools _tools = new Tools();
        object Data = new MachineDto();

        public MqttClientService(ManagedMqttClientOptions options,
            IServiceScopeFactory service, ILogger<MqttClientService> logger, IConfiguration configuration)
        {
            //this.hubContext = hubContext;
            this.options = options;
            _managedMqttClient = new MqttFactory().CreateManagedMqttClient();
            ConfigureMqttClient();
            _configuration = configuration;
        }

        private void ConfigureMqttClient()
        {
            _managedMqttClient.ConnectedAsync += HandleConnectedAsync;
            _managedMqttClient.DisconnectedAsync += HandleDisconnectedAsync;
            _managedMqttClient.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;
        }

        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string topic = eventArgs.ApplicationMessage.Topic;
            var payload = System.Text.Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);

            //Console.WriteLine(JsonSerializer.Serialize(payload));

            //if (topic.Contains("STATUSMACHINE"))
            //{
            //    using var connection = new NpgsqlConnection(_configuration.GetConnectionString("sqlConnection"));

            //    var payloadDeserialze = JsonSerializer.Deserialize<ModelMqttReceive>
            //    (payload, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            //    foreach (var item in payloadDeserialze.Values)
            //    {
            //        if (item.Quality != true)
            //        {
            //            return;
            //        }
            //        var svid = _tools.SplitData(item.Vid, 2, '_');

            //        if (connection.State == System.Data.ConnectionState.Closed) await connection.OpenAsync();
            //        var GetData = await connection.QueryAsync<GetDataDto>(@"SELECT b.name, c.name as category " +
            //            " FROM \"CategoryMachineHasMachine\" as a LEFT JOIN \"Machines\" as b on (a.\"MachineId\" = b.id)" +
            //            " LEFT JOIN \"CategoryMachines\" as c ON (a.\"CategoryMachineId\" = c.id) WHERE b.vid = @Vid ", new { Vid = svid });
            //        if (GetData.Count() != 0)
            //        {
            //            foreach (var mc in GetData)
            //            {
            //                Data = new 
            //                {
            //                    id = item.Vid,
            //                    name = mc.name,
            //                    category_machine = mc.category,
            //                    value = Convert.ToInt16(item.Value.ToString()),
            //                    date_time = DateTimeOffset.FromUnixTimeMilliseconds(item.Time).DateTime.ToString("yyyy-MM-dd HH:mm:ss",
            //                    CultureInfo.InvariantCulture),
            //                };

            //                try
            //                {
            //                    connection.Execute(@"INSERT INTO ""statusmachines"" VALUES (@Id, @Vid, @Name, @Value, " +
            //                        " @Date_time::TIMESTAMP WITH time zone)",
            //                    new
            //                    {
            //                        Id = item.Vid.ToString(),
            //                        Vid = svid,
            //                        Name = mc.name,
            //                        Value = Convert.ToInt16(item.Value.ToString()),
            //                        Date_time = DateTimeOffset.FromUnixTimeMilliseconds(item.Time).DateTime.ToString("yyyy-MM-dd HH:mm:ss",
            //                        CultureInfo.InvariantCulture),
            //                    });
            //                    Console.WriteLine("");
            //                }catch (Exception ex) 
            //                {
            //                    Console.WriteLine(ex.Message);
            //                }
            //            }
            //        }
            //        if (connection.State == System.Data.ConnectionState.Open) await connection.CloseAsync();
            //    }
            //}

            await Task.CompletedTask;

            //await hubContext.Clients.All.Broadcast(eventArgs.ApplicationMessage.Topic, payload);
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            //await hubContext.Clients.All.AgentConnectionStatus(true);
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            //await hubContext.Clients.All.AgentConnectionStatus(false);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _managedMqttClient.StartAsync(options);
            await _managedMqttClient.SubscribeAsync("#");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _managedMqttClient.StopAsync();
        }

        public async Task PublishAsync(string topic, string payload)
        {
            var applicationMessage = new MqttApplicationMessageBuilder().WithTopic(topic)
                                                                        .WithPayload(payload)
                                                                        .Build();
            var manegedApplicationMessage = new ManagedMqttApplicationMessageBuilder().WithApplicationMessage(applicationMessage)
                                                                                      .Build();
            await _managedMqttClient.EnqueueAsync(manegedApplicationMessage);
        }

        public IObservable<IEnumerable<MachineDto>> Observe()
        {
            return (IObservable<IEnumerable<MachineDto>>)Data;
        }
    }
}
