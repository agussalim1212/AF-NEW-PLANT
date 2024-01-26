using Dapper;
using SkeletonApi.Application.DTOs.RestApiData;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.Persistence.Interfaces;
using SkeletonApi.Persistence.Repositories.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IGenericRepository<Setting> _RepoMachine;
        private readonly IDapperCreateUnitOfWork _dapperUwow;
        private readonly IGetConnection _getConnection;
        public NotificationRepository(IGenericRepository<Setting> RepoMachine, DapperUnitOfWorkContext dapperUwow)
        {
            _RepoMachine = RepoMachine;
            _dapperUwow = dapperUwow;
            _getConnection = dapperUwow;
        }
        public async Task<IEnumerable<Setting>> GetAllSettingAsync() => await _RepoMachine.GetAllAsync();
        public async Task Creates(IEnumerable<Notifications> mqttRawValues)
        {
            try
            {
                using (var uwow = _dapperUwow.Create())
                {
                    var connection = _getConnection.GetConnection();

                    foreach (var row in mqttRawValues)
                    {
                        var notification = new Notifications();
                        notification.Id = Guid.NewGuid();
                        notification.CreatedAt = DateTime.UtcNow;
                        notification.Status = false;
                        notification.MachineName = row.MachineName;
                        notification.DateTime = row.DateTime;
                        notification.Message = row.Message;

                        string query = @"insert into ""Notifications"" (id,machine_name,message,status,created_at,date_time) values (@Id,@MachineName,@Message,@Status,@CreatedAt,@DateTime)";
                        await connection.ExecuteAsync(query, notification);
                           
                        
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
