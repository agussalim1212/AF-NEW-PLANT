using Dapper;
using SkeletonApi.Application.DTOs.RestApiData;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Application.Interfaces.Repositories.Dapper;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.Persistence.Interfaces;
using System.Text.Json;

namespace SkeletonApi.Persistence.Repositories.Dapper
{
    public class EnginePartRepository : IEnginePartRepository
    {
        private readonly IDapperCreateUnitOfWork _dapperUwow;
        private readonly IGetConnection _getConnection;
        private readonly IRestApiClientService _restApiClient;
        public EnginePartRepository(DapperUnitOfWorkContext dapperUwow,IRestApiClientService restClient)
        {
            _dapperUwow = dapperUwow;
            _getConnection = dapperUwow;
            _restApiClient = restClient;
        }

        public async Task Creates(IEnumerable<EnginePart> engineParts)
        {
            //await Console.Out.WriteLineAsync("Masuk SINI");
            try
            {
                using (var uwow = _dapperUwow.Create())
                {
                    var connection = _getConnection.GetConnection();
                    var query = @"insert into ""EngineParts"" (engine_id,torsi,abs,foto_data_ng,oil_brake,coolant,status,date_time) values (@engineid,@torsi,@abs,@fotodatang,@oilbrake,@coolant,@status,@datetime)";
                    await connection.ExecuteAsync(query, engineParts);
                    await uwow.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(JsonSerializer.Serialize(ex.Message));
            }

            await Console.Out.WriteLineAsync("Masuk SINI");
            await Task.CompletedTask;
        }

        public async Task Creates(IEnumerable<MqttRawValueEntity> mqttRawValues)
        {
            //await Console.Out.WriteLineAsync("Masuk SINI");
            
            try
            {
                using (var uwow = _dapperUwow.Create())
                {
                    var connection = _getConnection.GetConnection();
                    //string query;
                    foreach (var row in mqttRawValues)
                    {
                        var data = row.Value.ToString().ConvertCustomStringArrayToArray();
                        var enginePart = new EnginePartRaw();
                        enginePart.masterPartId = data[0];
                        enginePart.partId1 = data[1];
                        if (data.Count() > 2)
                            enginePart.partId2 = data[2];
                        enginePart.Datetime = row.Datetime;
                        var sample = new RestDataTraceability()
                        {
                            Id = row.Vid,
                            Value = row.Value,
                            Time = row.Time,
                            Quality = row.Quality,
                        };
                        await _restApiClient.SendAsync(sample);
                        //string query;
                        switch (row.Vid)

                        {
                            //string query = "ss";
                            case string a when a.Contains("NR-STR-STEM"):
                                {
                                    string query = @"insert into ""EngineParts"" (engine_id,torsi,date_time) values (@masterPartId,@partId1,@datetime)";
                                    await connection.ExecuteAsync(query, enginePart);
                                    break;
                                }

                            case string a when a.Contains("PRESS-FIT_ID-Part"):
                                {
                                    string query = @"update ""EngineParts"" set cr_shft = @partId1,date_time = @datetime where cr_cs_l = @masterPartId";
                                    await connection.ExecuteAsync(query, enginePart);
                                    break;
                                }
                            case string a when a.Contains("SINCRONIZE_ID-Part"):
                                {
                                    string query = @"update ""EngineParts"" set cr_cs_r = @partId1,date_time = @datetime where cr_cs_l = @masterPartId";
                                    await connection.ExecuteAsync(query, enginePart);
                                    break;
                                }
                            case string a when a.Contains("POLLING_ID-Updater1"):
                                {
                                    string query = @"update ""EngineParts"" set cyl_comp = @partId1, cyl_head = @partId2,date_time = @datetime where engine_id = @masterPartId";
                                    await connection.ExecuteAsync(query, enginePart);
                                    break;
                                }
                            case string a when a.Contains("POLLING_ID-Repair1"):
                                {
                                    var query1 = await connection.QueryFirstOrDefaultAsync<EnginePart>(@"select * from ""EngineParts"" where engine_id = @id", new { id = enginePart.masterPartId });
                                    query1.Status = "RP";
                                    var query2 = @"insert into ""EngineParts"" (engine_id,cyl_head,cr_cs_l,cr_cs_r,cyl_comp,cr_shft,status,date_time) values (@engineid,@cylhead,@crcsl,@crcsr,@cylcomp,@crshft,@status,@datetime)";
                                    await connection.ExecuteAsync(query2, query1);

                                    string query = @"update ""EngineParts"" set cyl_comp = @partId1, cyl_head = @partId2,date_time = @datetime where engine_id = @masterPartId AND status = NULL";
                                    await connection.ExecuteAsync(query, enginePart);
                                    break;
                                }
                            case string a when a.Contains("POLLING_ID-Repair2"):
                                {
                                    var query1 = await connection.QueryFirstOrDefaultAsync<EnginePart>(@"select * from ""EngineParts"" where engine_id = @id", new { id = enginePart.masterPartId });
                                    query1.Status = "RP";
                                    var query2 = @"insert into ""EngineParts"" (engine_id,cyl_head,cr_cs_l,cr_cs_r,cyl_comp,cr_shft,status,date_time) values (@engineid,@cylhead,@crcsl,@crcsr,@cylcomp,@crshft,@status,@datetime)";
                                    await connection.ExecuteAsync(query2, query1);

                                    string query = @"update ""EngineParts"" set cyl_comp = @partId1, cyl_head = @partId2,date_time = @datetime where engine_id = @masterPartId AND status = NULL";
                                    await connection.ExecuteAsync(query, enginePart);
                                    break;
                                }
                            case string a when a.Contains("POLLING_ID-Final"):
                                {
                                    string query = @"update ""EngineParts"" set cyl_comp = @partId1, cyl_head = @partId2,date_time = @datetime, status = 'OK' where engine_id = @masterPartId AND status = NULL";
                                    await connection.ExecuteAsync(query, enginePart);
                                    break;
                                }
                            default:
                                //var query = @"insert into ""EngineParts"" (engine_id,cyl_head,cr_cs_l,cr_cs_r,cyl_comp,cr_shft,status,date_time) values (@engineid,@cylhead,@crcsl,@crcsr,@cylcomp,@crshft,@status,@datetime)";
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

        public async Task<IEnumerable<EnginePart>> FindById(string id)
        {
            try
            {
                using (var uwow = _dapperUwow.Create())
                {
                    var connection = _getConnection.GetConnection();
                    var data = await connection.QueryAsync<EnginePart>(@"select * from ""EngineParts"" where engine_id = id", id);
                    connection.Close();
                    return data;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(JsonSerializer.Serialize(ex.Message));
                return null;
            }
        }
    }

    public static class ExtensionHelper
    {
        //public static string GetReverseString(this string str)
        //{
        //    char[] chars = str.ToCharArray();
        //    char temp;
        //    for (int i = 1; i < chars.Length; i += 2)
        //    {
        //        temp = chars[i];
        //        chars[i] = chars[i - 1];
        //        chars[i - 1] = temp;
        //    }
        //    string alternateReversedString = new string(chars);
        //    return alternateReversedString;
        //}
        //public static string RemoveUnicode(this string arg)
        //{
        //    var a = arg.GetReverseString();
        //    StringBuilder sb = new StringBuilder(arg.Length);
        //    foreach (char c in a)
        //    {
        //        if ((int)c > 127) // you probably don't want 127 either
        //            continue;
        //        if ((int)c < 32)  // I bet you don't want control characters
        //            continue;
        //        if (c == '%')
        //            continue;
        //        if (c == '?')
        //            continue;
        //        sb.Append(c);
        //    }
        //    var CleanString = Regex.Replace(sb.ToString(), @"\s{2,}", ".").TrimEnd(' ');
        //    return CleanString.Remove(CleanString.Length - 1, 1);
        //}

        public static string[] ConvertCustomStringArrayToArray(this string input)
        {
            // Langkah 1: Hapus tanda kurung siku
            string cleanInput = input.Trim('[', ']');

            // Langkah 2: Pisahkan string menjadi elemen-elemen
            string[] elements = cleanInput.Split(',');

            // Langkah 3: Bersihkan elemen-elemen
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = elements[i].Trim();
            }

            // Hasil akhir
            return elements;
        }
    }
}