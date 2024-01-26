using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Configuration.Dapper;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.IotHub.DTOs;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
using SkeletonApi.IotHub.Services.Store;
using System.Reactive;

namespace SkeletonApi.IotHub.Services
{
    public class NotificationConsumer : BackgroundService
    {
        private readonly IIoTHubEventHandler<MqttRawDataEncapsulation> _mqttStoreEventHandler;
        private readonly IotHubNotificationEventHandler _notificationEventHandler;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly NotificationStore _notificationStore;
        private readonly SubjectStore _subjectStore;

        public NotificationConsumer(IIoTHubEventHandler<MqttRawDataEncapsulation> mqttStoreEventHandler,
            IServiceScopeFactory serviceScopeFactory,
            IMapper mapper,
            IotHubNotificationEventHandler notificationEventHandler,
            NotificationStore notificationStore,
            SubjectStore subjectStore
            
            )
        {
            _notificationStore = notificationStore;
            _mqttStoreEventHandler = mqttStoreEventHandler;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _notificationEventHandler = notificationEventHandler;
            _subjectStore = subjectStore;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _mqttStoreEventHandler.Subscribe(
                subscriberName: typeof(NotificationConsumer).Name,
                action: async (val) =>
                {
                    if (val.mqttRawData != null && val.topics != null)
                    {
                        await PersistNotificationToDBAsync(val.mqttRawData);                        
                    }
                });

            return Task.CompletedTask;
        }

        private async Task PersistNotificationToDBAsync(MqttRawData value) 
        {
            var Subject = _subjectStore.GetAllSubject();
          
            if (value.Values is not null)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var notificationList = from vls in value.Values.Where(x => x.Quality == true)
                                           join ids in Subject on vls.Vid equals ids.Vid
                                           where vls.Vid == ids.Vid
                                           group new { vls, ids } by vls.Vid into g
                                           orderby g.Key descending
                                           select new NotificationModel
                                           {
                                               MachineName = g.Last().ids.Subjects,
                                               Message = g.Last().vls.Value.ToString(),
                                               Datetime = DateTimeOffset.FromUnixTimeMilliseconds(g.Last().vls.Time).DateTime
                                               
                                           };
                    var notification = _notificationStore.GetAllSetting().Where(x => (x.SubjectName == notificationList.FirstOrDefault().MachineName 
                    && Convert.ToDecimal(notificationList.FirstOrDefault().Message) > x.Maximum) ||
                    (x.SubjectName == notificationList.FirstOrDefault().MachineName
                    && Convert.ToDecimal(notificationList.FirstOrDefault().Message) < x.Minimum));

                    var dataNotification = notificationList.Select(g => new NotificationModel
                           {
                                MachineName = g.MachineName,
                                Message = $"ABNORMAL VALUE, CURRENT VALUE IS {g.Message}",
                                Datetime = g.Datetime,
                                Status = false
                          });
                    _notificationEventHandler.Dispatch(dataNotification);

                    var scoped = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                    var mqttRawValueEntities = _mapper.Map<IEnumerable<Notifications>>(dataNotification);
                    scoped.Creates(mqttRawValueEntities);
                }
            }
        }

    }
}
