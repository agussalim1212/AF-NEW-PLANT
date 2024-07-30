using AutoMapper;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
using SkeletonApi.IotHub.Services.Store;

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
                try
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
                        foreach(var notif in notificationList)
                        {
                            var dt = new List<NotificationModel>();
                            var notification = _notificationStore.GetAllSetting().Where(x => (x.SubjectName == notif.MachineName
                            && Convert.ToDecimal(notif.Message) > x.Maximum && notif.Message != "0") ||
                            (x.SubjectName == notif.MachineName && Convert.ToDecimal(notif.Message) < x.Minimum
                            && notif.Message != "0"));

                            if (notification.Count() != 0)
                            {
                                var dataNotification = new NotificationModel
                                {
                                    MachineName = notif.MachineName,
                                    Message = $"ABNORMAL VALUE, CURRENT VALUE IS {notif.Message} IN {notif.MachineName}",
                                    Datetime = notif.Datetime,
                                    Status = false
                                };
                                dt.Add(dataNotification);
                                _notificationEventHandler.Dispatch(dt);

                                var scoped = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                                var mqttRawValueEntities = _mapper.Map<Notifications>(dataNotification);
                                scoped.Create(mqttRawValueEntities);
                            }
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
}