using Dapper;
using SkeletonApi.Application.DTOs.RestApiData;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Application.Interfaces.Repositories.Configuration.Dapper;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Repositories.Dapper
{
    public class DeviceDataRepository : IDiviceDateRepository
    {
        private readonly IDapperCreateUnitOfWork _dapperUwow;
        private readonly IGetConnection _getConnection;
        private readonly IRestApiClientService _restApiClient;

        public DeviceDataRepository (DapperUnitOfWorkContext dapperUwow, IRestApiClientService restClient)
        {
            _dapperUwow = dapperUwow;
            _getConnection = dapperUwow;
            _restApiClient = restClient;
        }

        public async Task Creates(IEnumerable<MqttRawValueEntity> mqttRawValues)
        {
            try
            {
                using (var uwow = _dapperUwow.Create())
                {
                    var connection = _getConnection.GetConnection();

                    foreach (var row in mqttRawValues)
                    {
                        var deviceData = new DeviceData();
                        deviceData.Id = row.Vid;
                        deviceData.Value = Convert.ToString(row.Value);
                        deviceData.Quality = row.Quality;
                        deviceData.Time = row.Time;
                        deviceData.DateTime = row.Datetime;
                        var sample = new RestDataTraceability()
                        {
                            Id = row.Vid,
                            Value = row.Value,
                            Time = row.Time,
                            Quality = row.Quality,
                        };
                        switch (row.Vid)

                        {
                            case string a when a.Contains("AIR-CONSUMPTION"):
                                {
                                    string query = @"insert into ""AirConsumptions"" (id,value,quality,time,date_time) values (@Id,@Value,@Quality,@Time,@DateTime)";
                                    await connection.ExecuteAsync(query, deviceData);
                                    break;
                                }
                            case string a when a.Contains("POWER-CONSUMPTION"):
                                {
                                    string query = @"insert into ""PowerConsumptions"" (id,value,quality,time,date_time) values (@Id,@Value,@Quality,@Time,@DateTime)";
                                    await connection.ExecuteAsync(query, deviceData);
                                    break;
                                }
                            case string a when a.Contains("ELECT-GNTR"):
                                {
                                    string query = @"insert into ""ElectGntrs"" (id,value,quality,time,date_time) values (@Id,@Value,@Quality,@Time,@DateTime)";
                                    await connection.ExecuteAsync(query, deviceData);
                                    break;
                                }
                            case string a when a.Contains("COUNT-PRDCT"):
                                {
                                    string query = @"insert into ""TotalProductions"" (id,value,quality,time,date_time) values (@Id,@Value,@Quality,@Time,@DateTime)";
                                    await connection.ExecuteAsync(query, deviceData);
                                    break;
                                }
                            case string a when a.Contains("OFF-TIME"):
                                {
                                    string query = @"insert into ""StopLines"" (id,value,quality,time,date_time) values (@Id,@Value,@Quality,@Time,@DateTime)";
                                    await connection.ExecuteAsync(query, deviceData);
                                    break;
                                }
                            case string a when a.Contains("CYCLE-COUNT") || a.Contains("RUN-TIME") ||  a.Contains("RIM"):
                                {
                                    string query = @"insert into ""MachineInformation"" (id,value,quality,time,date_time) values (@Id,@Value,@Quality,@Time,@DateTime)";
                                    await connection.ExecuteAsync(query, deviceData);
                                    break;
                                }
                            case string a when a.Contains("STATUS-PRDCT") || a.Contains("ID-PART") || a.Contains("TORQ") || a.Contains("FRQ-INVERT") || a.Contains("TIME-OPARATION")
                                || a.Contains("VOL") || a.Contains("LEAK-TES") || a.Contains("CODE") || a.Contains("IMAG-NG") || a.Contains("DEPTH") || a.Contains("TONASE")
                                || a.Contains("DISTANCE") || a.Contains("DIAL") || a.Contains("TIRE-PRESURE"):
                                {
                                    string query = @"insert into ""ListQualities"" (id,value,quality,time,date_time) values (@Id,@Value,@Quality,@Time,@DateTime)";
                                    await connection.ExecuteAsync(query, deviceData);
                                    break;
                                }
                            default:
                                await Console.Out.WriteLineAsync("NULL");
                                break;
                        }
                    }
                    await uwow.CommitAsync();
                    uwow.Dispose();
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(JsonSerializer.Serialize(ex.Message));
            }

            await Task.CompletedTask;
        }

    }
}
